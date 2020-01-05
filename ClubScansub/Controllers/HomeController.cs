using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClubScansub.Models;
using ClubScansub.Data;
using ClubScansub.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ClubScansub.Extensions;
using Microsoft.AspNetCore.Authorization;
using Nager.Date;
using ClubScansub.Utility;
using ClubScansub.Service;
using Microsoft.AspNetCore.Identity;

namespace ClubScansub.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISmsSender smsSender;
        private readonly UserManager<IdentityUser> um;
        private List<string> DefaultClassNames;

        public HomeController(ApplicationDbContext db,ISmsSender smsSender, UserManager<IdentityUser> um )
            :base(db)
        {
            this.smsSender = smsSender;
            this.um = um;

            DefaultClassNames = new List<string>() { "calendar-event", "border", "rounded", "p-2", "small" };
        }

        [TempData]
        public string StatusMessage { get; set; }

        public HomeIndexViewModel PageModel { get; set; }
        public async Task<IActionResult> Index(string showcalendar = "trips")
        {
            PageModel = new HomeIndexViewModel()
            {
                UpcomingEvents = await db.Events
                    .Include(x=>x.Participants)
                        .ThenInclude(x=>x.ApplicationUser)
                    .Include(x => x.Divelocation)
                        .ThenInclude(x=>x.MeetingLocation)
                    .Where(x => x.StartDateAndTime > DateTime.Now)
                    .OrderBy(x=>x.StartDateAndTime)
                    .ToListAsync(),
                Statusmessage = StatusMessage,
                CalendarType = showcalendar
            };
            return View(PageModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents(DateTime start, DateTime end, EventTypeEnum eventtype)
        {
            

            switch (eventtype)
            {
                case EventTypeEnum.Bådtur:
                    DefaultClassNames.Add("bg-boat");
                    break;
                case EventTypeEnum.Stranddyk:
                    DefaultClassNames.Add("bg-beach");
                    break;
                case EventTypeEnum.Rejse:
                    DefaultClassNames.Add("bg-travel");
                    break;
                case EventTypeEnum.Liveaboard:
                    DefaultClassNames.Add("bg-liveaboard");
                    break;
                case EventTypeEnum.Klubture:
                    DefaultClassNames.Add("bg-club");
                    break;
                case EventTypeEnum.Other:
                    DefaultClassNames.Add("bg-other");
                    break;
                default:
                    break;
            }

            var data = await db.Events
                    .Where(x => x.StartDateAndTime >= start && x.EndDateAndTime <= end && x.EventType == eventtype && !x.IsCancelled && !x.IsInternal)
                    .OrderBy(x => x.StartDateAndTime)
                    .ToListAsync();

            var result = data.Select(v => new {
                id = $"eventId:{v.Id}",
                title = v.Title,
                description = v.Details,
                start = v.StartDateAndTime.ToString("yyyy-MM-dd hh:mm:ss"),
                end = v.EndDateAndTime.ToString("yyyy-MM-dd hh:mm:ss"),
                classNames = DefaultClassNames.ToArray()
            }); 

            return new JsonResult(result);//, JsonRequestBehavior = }
        }

        [HttpGet]
        public async Task<JsonResult> GetInternalEvents(DateTime start, DateTime end)
        {
            DefaultClassNames.Add("bg-internal");

            var data = await db.Events
                    .Where(x => x.StartDateAndTime >= start && x.EndDateAndTime <= end && !x.IsCancelled && x.IsInternal)
                    .OrderBy(x => x.StartDateAndTime)
                    .ToListAsync();

            var result = data.Select(v => new {
                id = $"eventId:{v.Id}",
                title = v.Title,
                description = v.Details,
                start = v.StartDateAndTime.ToString("yyyy-MM-dd hh:mm:ss"),
                end = v.EndDateAndTime.ToString("yyyy-MM-dd hh:mm:ss"),
                classNames = DefaultClassNames.ToArray()
            });

            return new JsonResult(result);//, JsonRequestBehavior = }
        }

        [HttpGet]
        public async Task<JsonResult> GetCourses(DateTime start, DateTime end, CourseTypeEnum courseType)
        {

            var sessions = await db.CourseSessions.Include(x => x.Course).Where(x => x.DateTime >= start && x.DateTime <= end && x.Course.CourseType == courseType).OrderBy(x=>x.DateTime).ToListAsync();

            switch (courseType)
            {
                case CourseTypeEnum.BaseCourse:
                    DefaultClassNames.Add("bg-basecourse");
                    break;
                case CourseTypeEnum.TechCourse:
                    DefaultClassNames.Add("bg-techcourse");
                    break;
                case CourseTypeEnum.ProCourse:
                    DefaultClassNames.Add("bg-procourse");
                    break;
                case CourseTypeEnum.SpecialtyCourse:
                    DefaultClassNames.Add("bg-speccourse");
                    break;
                default:
                    break;
            }

            var result = sessions.Select(v => new {
                id = $"sessionId:{v.Id}",
                title = $"{v.Course.CourseName} ({v.Sessiontype.ToString()})",
                description = v.Sessiontype,
                start = v.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                end = v.DateTime.Add(v.Duration).ToString("yyyy-MM-dd HH:mm:ss"),
                classNames = DefaultClassNames.ToArray()
            });

            return new JsonResult(result);//, JsonRequestBehavior = }
        }

        [HttpGet]
        public async Task<JsonResult> GetHolidays(DateTime start, DateTime end)
        {
            //Get all publicHolidays for a date range
            var publicHolidays = DateSystem.GetPublicHoliday(start, end, CountryCode.DK);

            var result  = await  Task.Run( () => {

                return publicHolidays.Select(x => new
                {
                    title = x.LocalName,
                    allDay = true,
                    start = x.Date,
                    end = x.Date,
                    rendering = "background"
                });
               
            });

            return new JsonResult(result);
            //foreach (var publicHoliday in publicHolidays)
            //{
            //    //publicHoliday...
            //    //publicHoliday.Date -> The date
            //    //publicHoliday.LocalName -> The local name
            //    //publicHoliday.Name -> The english name
            //    //publicHoliday.Fixed -> Is this public holiday every year on the same date
            //    //publicHoliday.Global -> Is this public holiday in every county (federal state)
            //    //publicHoliday.Counties -> Is the public holiday only valid for a special county ISO-3166-2 - Federal states
            //    //publicHoliday.Type -> Public, Bank, School, Authorities, Optional, Observance
            //}
        }

        [HttpGet]
        public async Task<IActionResult> GetEventDetails(string id)
        {
            var idItems = id.Split(":");
            int itemId = int.Parse(idItems[1]);
            string itemType = idItems[0];

            if (itemType == "sessionId")
            {
                var session = await db.CourseSessions.Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id == itemId);

                var course = await db.Courses
                    .Include(x => x.CourseSessions).ThenInclude(x=>x.CourseSessionTemplate)
                    .Include(x => x.CourseTemplate)
                    .Include(x=>x.Signups).ThenInclude(x=>x.ApplicationUser)
                    .Include(x=>x.Participants).ThenInclude(x => x.ApplicationUser)
                    .FirstOrDefaultAsync(x => x.Id == session.Course.Id);

                return PartialView("_CourseDetailsPartial", course);
            }
            else
            {

                var @event = await db.Events
                        .Include(x => x.RequiredCertificate)
                        .Include(x => x.Participants)
                            .ThenInclude(x => x.ApplicationUser)
                        .Include(x => x.Divelocation)
                            .ThenInclude(x => x.MeetingLocation)
                        .FirstOrDefaultAsync(x => x.Id == itemId);
                return PartialView("_EventDetailsPartial", @event);
            }   

        }

        [Authorize]
        public async Task<IActionResult> Signup(int id)
        {
            var item = await db.Events.Include(x=>x.Participants).Include(x=>x.RequiredCertificate).FirstAsync(x=>x.Id==id);
            var user = await db.ApplicationUsers.Include(x=>x.AccountTransactions).Include(x=>x.Certificates).ThenInclude(x=>x.Certificate).FirstOrDefaultAsync(x=>x.Id == User.GetIdentityId());

            if (item == null || user == null)
            {
                StatusMessage = $"Der skete en fejl: Bruger eller begivenhed ikke genkendt.";
                return RedirectToAction(nameof(Index));
            }

            var isPaymentRequired = !((item.EventType == EventTypeEnum.Stranddyk || item.EventType == EventTypeEnum.Other) && User.IsInRole(Userroles.Member));
            
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

            StatusMessage = $"Du er blevet tilmeldt '{item.Title}'";

            if (item.RequiredCertificate != null && !user.Certificates.Any(x => x.Certificate.Id == item.RequiredCertificate.Id))
                StatusMessage += $" men - ADVARSEL: Du har ikke det krævede certifikat '{item.RequiredCertificate.Name}', så det kan blive en lang, trist dag uden dykning. Sørg for at opdatere dine certifikater under din profil og medbring dit certifikat til turen.";

            return RedirectToAction(nameof(Index));

        }

        [Authorize]
        public async Task<IActionResult> Attend(int id)
        {
            var course = await db.Courses.FindAsync(id);
            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());
            
            var siteowners = await um.GetUsersInRoleAsync(Userroles.Owner);
            var admins = await um.GetUsersInRoleAsync(Userroles.Administrator);

            var coursesignee = new CourseSignup() { ApplicationUser = user };

            course.Signups.Add(coursesignee);

            await db.SaveChangesAsync();

            StatusMessage = $"Du er blevet skrevet op til kurset '{course.CourseName}'. Scansub DK Diver vil kontakte dig for yderligere informationer.";

            var smsText = $"Der er kommet en ny tilmelding til kurset {course.CourseName} fra {user.Name}. ";

            if (user.PhoneNumberConfirmed)
            {
                var smsSendResult = await smsSender.SendSmsAsync(user.PhoneNumber, StatusMessage);
                smsText += $"Kontakt på telefon {user.PhoneNumber}";
            }
            else //if (user.EmailConfirmed)
            {
                smsText += $"Kontakt på email {user.UserName}";
            }
            //else
            //{
            //    smsText += ", men vedkommende har hverken valideret telefon eller email.";
            //}

            foreach (var item in siteowners)
            {
                if (item.PhoneNumberConfirmed)
                {
                    var smsSendResult = await smsSender.SendSmsAsync(item.PhoneNumber, smsText);
                }
            }
            foreach (var item in admins.Where(x=> !siteowners.Contains(x)))
            {
                if (item.PhoneNumberConfirmed)
                {
                    var smsSendResult = await smsSender.SendSmsAsync(item.PhoneNumber, smsText);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View(db.ClubSettings.SingleOrDefault());
        }
        public IActionResult Terms()
        {
            return View(db.ClubSettings.SingleOrDefault());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
