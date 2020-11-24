using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.API
{
    [Route("api")]
    [ApiController]
    public class EventsController : BaseController
    {
        private UserManager<IdentityUser> userManager;

        public EventsController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(db)
        {
            this.userManager = userManager;

        }
        #region Public end points
        
        [Route("events/{eventtype}")]
        public async Task<ActionResult> Get(string eventtype)
        {

            EventTypeEnum type = (EventTypeEnum)Enum.Parse(typeof(EventTypeEnum), eventtype);

            int monthsAhead = type == EventTypeEnum.Klubture ? 3 : 12;

            decimal balance = 0.0m;
            bool isSignedIn = false;
            bool isDivePro = false;
            bool userShouldPay = true;

            ApplicationUser user = null;
            ICollection<EventUser> userevents = new List<EventUser>();

            if (User.Identity.IsAuthenticated)
            {
                if (User.HasClaim("isPremium","True") && type== EventTypeEnum.Klubture)
                    userShouldPay = false;

                if (User.HasClaim("isDivePro", "True"))
                    isDivePro = true;

                if (userManager.GetUserId(User) != null)
                {
                    user = await db.ApplicationUsers
                        .Include(x => x.Events)
                        .Include(x => x.AccountTransactions)
                        .SingleAsync(x => x.Id == userManager.GetUserId(User));
                    balance = user.Balance;
                    isSignedIn = true;
                    userevents = user.Events;
                }
            }

            try
            {
                var data = await db.Events
                .Include(x => x.Divelocation).ThenInclude(x => x.MeetingLocation)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.IsCancelled == false &&
                        (x.MaxParticipants - x.FixedParticipants - x.Participants.Count) > 0 &&
                        (x.StartDateAndTime > DateTime.Now && x.StartDateAndTime < DateTime.Now.AddMonths(monthsAhead)) &&
                        (x.EventType == type))
                .OrderBy(x => x.StartDateAndTime)
                .Select(x => new
                {
                    Id = x.Id.ToString(),
                    Name = x.Title,
                    x.Details,
                    Date = x.StartDateAndTime,
                    //Time = x.StartDateAndTime.TimeOfDay.ToString(),
                    Participants = x.Participants.Count,
                    ParticipantUsers = x.Participants.Count > 0 ? x.Participants.Select(x => new { x.ApplicationUser.Name, x.ApplicationUser.PhoneNumber, x.ApplicationUser.Id, x.ApplicationUser.Email }) : null,
                    x.MinParticipants,
                    x.MaxParticipants,
                    x.FixedParticipants,
                    FreeSpots = x.MaxParticipants - x.FixedParticipants - x.Participants.Count,
                    RequiredSpots = (x.MinParticipants - x.FixedParticipants - x.Participants.Count) < 0 ? 0 : (x.MinParticipants - x.FixedParticipants - x.Participants.Count),
                    x.Divelocation.Image,
                    x.Divelocation.MeetingLocation,
                    x.Divelocation.Description,
                    x.Divelocation.DiveType,
                    x.Divelocation.MinDepth,
                    x.Divelocation.MaxDepth,
                    Price = userShouldPay ? x.Price : 0,
                    AlreadySignedUp = x.Participants.Any(p=> userevents.Contains(p)),
                    HasFunds = x.Price > balance && userShouldPay ? false : true,
                    isSignedIn,
                    isDivePro
                })
                .ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion

        #region Personal Endpoints - require autorization

        [Authorize]
        [Route("signup")]
        [HttpPost]
        public async Task<ActionResult> Signup([FromBody] SignupEventId model)
        {

            var Id = int.Parse(model.Id);

            var item = await db.Events.Include(x => x.Participants).Include(x => x.RequiredCertificate).FirstAsync(x => x.Id == Id);
            var user = await db.ApplicationUsers.Include(x => x.AccountTransactions).Include(x => x.Certificates).ThenInclude(x => x.Certificate).FirstOrDefaultAsync(x => x.Id == User.GetIdentityId());

            string StatusMessage;

            if (item == null || user == null)
            {
                StatusMessage = $"Der skete en fejl: Bruger eller begivenhed ikke genkendt.";
                return RedirectToAction(nameof(Index));
            }

            var isPaymentRequired = !((item.EventType == EventTypeEnum.Other || item.EventType == EventTypeEnum.Klubture) && User.IsInRole(Userroles.Member));

            if (item.IsFreeForDivepros && User.IsInRole(Userroles.Divepro))
            {
                isPaymentRequired = false;
            }

            if (item.Price > user.Balance && isPaymentRequired)
            {
                StatusMessage = "Fejl - der er ikke penge nok på din turkonto, du bliver nødt til at tanke op";
                return RedirectToAction(nameof(Index));
            }

            var accounttrans = new ApplicationUserAccountEntry
            {
                Amount = isPaymentRequired ? -item.Price : 0,
                AccountType = AccountTypeEnum.EventAccountType,
                Description = $"{item.Title} ,d. {item.StartDateAndTime.ToShortDateString()}",
                Event = item,
                PostingDate = DateTime.Now
            };

            user.AccountTransactions.Add(accounttrans);

            var eventUser = new EventUser() { ApplicationUser = user };

            item.Participants.Add(eventUser);
            await db.SaveChangesAsync();

            StatusMessage = $"Du er blevet tilmeldt '{item.Title}' d. {item.StartDateAndTime.ToShortDateString()}";

            if (item.RequiredCertificate != null && !user.Certificates.Any(x => x.Certificate.Id == item.RequiredCertificate.Id))
                StatusMessage += $" men - ADVARSEL: Du har ikke det krævede certifikat '{item.RequiredCertificate.Name}', så det kan blive en lang, trist dag uden dykning. Sørg for at opdatere dine certifikater under din profil og medbring dit certifikat til turen.";

            return Ok(StatusMessage);


        }
        public class SignupEventId
        {
            public string Id { get; set; }
        }

        [Authorize]
        [Route("myevents")]
        [HttpGet]
        public async Task<ActionResult> MyEvents()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await db.ApplicationUsers.FindAsync(userManager.GetUserId(User));
            if (user == null)
                return NotFound();

            var eventplan = await db.EventUsers
                .Include(x => x.Event).ThenInclude(x => x.Divelocation).ThenInclude(x => x.MeetingLocation)
                .Include(x=>x.Event).ThenInclude(x=>x.Participants)
                .Where(x => x.ApplicationUser == user && x.Event.StartDateAndTime > DateTime.Now)
                .ToListAsync();

            var result = eventplan.Select(x => new
            {
                Id = x.EventId.ToString(),
                Name = x.Event.Title,
                Date = x.Event.StartDateAndTime,
                Participants = x.Event.Participants.Count + x.Event.FixedParticipants,
                x.Event.MinParticipants,
                x.Event.MaxParticipants,
                x.Event.FixedParticipants,
                FreeSpots = x.Event.MaxParticipants - x.Event.FixedParticipants - x.Event.Participants.Count,
                RequiredSpots = (x.Event.MinParticipants - x.Event.FixedParticipants - x.Event.Participants.Count) < 0 ? 0 : (x.Event.MinParticipants - x.Event.FixedParticipants - x.Event.Participants.Count),
                x.Event.Divelocation.Image,
                x.Event.Divelocation.MeetingLocation,
                x.Event.Divelocation.MinDepth,
                x.Event.Divelocation.MaxDepth
            });

            return Json(result);


        }

        [Authorize]
        [Route("mycourses")]
        [HttpGet]
        public async Task<ActionResult> MyCourses()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await db.ApplicationUsers.FindAsync(userManager.GetUserId(User));
            if (user == null)
                return NotFound();

            var courses = await db.CourseSignups
                .Include(x => x.Course)
                .Where(x => x.ApplicationUser == user && x.Course.EndDateAndTime > DateTime.Now)
                .Select(x => x.Course)
                .ToListAsync();

            var result = await db.CourseSessions.Where(x => courses.Contains(x.Course) && x.DateTime > DateTime.Now)
                .ToListAsync();

            return Json(result);
        }


        [Authorize]
        [Route("myinstructorsessions")]
        [HttpGet]
        public async Task<ActionResult> MyInstructorSessions()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await db.ApplicationUsers.FindAsync(userManager.GetUserId(User));
            if (user == null)
                return NotFound();


            var workplan = await db.CourseSessionInstructors
                 .Include(x => x.CourseSession).ThenInclude(x => x.Course)
                 .Include(x => x.CourseSession).ThenInclude(x => x.CourseSessionTemplate)
                 .Where(x => x.InstructorId == User.GetIdentityId() && x.CourseSession.DateTime > DateTime.Now)
                 .ToListAsync();

            var result = workplan.Select(
                x => new
                {
                    Id = x.CourseSessionId.ToString(),
                    x.CourseSession.DateTime,
                    x.CourseSession.SessionDescription,
                    x.CourseSession.Divelocation,
                    x.CourseSession.Course
                }
                );

            return Json(result);

        }

        #endregion
    }
}
