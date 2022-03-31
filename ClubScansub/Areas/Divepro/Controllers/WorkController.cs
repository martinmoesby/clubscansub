using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Divepro.Controllers
{
    [Area("Divepro")]
    public class WorkController : BaseDiveproController
    {
        private ISmsSender smsSender;
        private UserManager<IdentityUser> userManager;

        public WorkController(ApplicationDbContext db, ISmsSender smsSender, UserManager<IdentityUser> userManager)
            : base(db)
        {
            this.smsSender = smsSender;
            this.userManager = userManager;
            CurrentMonth = DateTime.Now.Month;
            CurrentYear = DateTime.Now.Year;
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public int ActiveCourse { get; set; }

        [TempData]
        public int CurrentMonth { get; set; }

        [TempData]
        public int CurrentYear { get; set; }

        public async Task<IActionResult> Index()
        {
            ViewBag.Pagetitle = "Kurser der mangler instruktøer";
            ViewBag.StatusMessage = StatusMessage;
            ViewBag.ActiveCourse = ActiveCourse;

            var myProCerts = db.UserCertificates
                .Include(x => x.User)
                .Include(x=>x.Certificate)
                .Where(x => x.User.Id == User.GetIdentityId() && x.Certificate.IsDiveproCertificate == true);

            var courses = await db.Courses
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.SessionInstructors)
                        .ThenInclude(x => x.Instructor)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.CourseSessionTemplate)
                .Include(x=>x.Signups)
                    .ThenInclude(x=>x.ApplicationUser)
                .Include(x=>x.Participants)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.CourseTemplate)
                    .ThenInclude(x=>x.InstructorCertificate)
                .Where(x => x.EndDateAndTime > DateTime.Now && myProCerts.Any(y => y.Certificate.Id == x.CourseTemplate.InstructorCertificate.Id))
                .OrderBy(x=>x.StartDateAndTime)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Workcalendar(int offset = 0)
        {
            ViewBag.Pagetitle = "Divepro Arbejdskalender";
            CurrentMonth += offset;
            if (CurrentMonth < 1)
            {
                CurrentMonth = 12;
                CurrentYear--;
            }

            if (CurrentMonth> 12)
            {
                CurrentMonth = 1;
                CurrentYear++;

            }

            ViewBag.StatusMessage = StatusMessage;
            ViewBag.ActiveCourse = ActiveCourse;
            ViewBag.CurrentMonth = CurrentMonth;
            ViewBag.CurrentYear = CurrentYear;

            var myProCerts = db.UserCertificates
                .Include(x => x.User)
                .Include(x => x.Certificate)
                .Where(x => x.User.Id == User.GetIdentityId() && x.Certificate.IsDiveproCertificate == true);

            var courses = await db.CourseSessions
                .Include(x => x.SessionInstructors)
                        .ThenInclude(x => x.Instructor)
                .Include(x => x.CourseSessionTemplate)
                .Include(x=>x.Course)
                    .ThenInclude(x => x.Signups)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Course)
                    .ThenInclude(x => x.Participants)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.Course)
                    .ThenInclude(x => x.CourseTemplate)
                        .ThenInclude(x => x.InstructorCertificate)
                .Where(x => x.DateTime.Month == CurrentMonth && x.DateTime.Year == CurrentYear && myProCerts.Any(y => y.Certificate.Id == x.Course.CourseTemplate.InstructorCertificate.Id))
                .AsNoTracking()
                .ToListAsync();

            return View(courses);
        }

        [Authorize(Roles = Userroles.Divepro)]
        public async Task<IActionResult> Apply(int[] sessionid)
        {
            string sessionDescription = "Du har skrevet dig på som instruktør til: \n";

            foreach (var item in sessionid)
            {

                var session = await db.CourseSessions
                    .Include(x=>x.Course)
                    .Include(x => x.CourseSessionTemplate)
                    .Include(x => x.SessionInstructors)
                    .FirstOrDefaultAsync(x => x.Id == item);

                ActiveCourse = session.Course.Id;

                session.SessionInstructors.Add(new CourseSessionInstructor() { InstructorId = User.GetIdentityId() });

                var sessionText = session.CourseSessionTemplate != null ? session.CourseSessionTemplate.Name : session.SessionName;
                    sessionDescription += $"{sessionText} d. {session.DateTime.ToString("dd. MMMM yyyy")} \n";

            }
            await db.SaveChangesAsync();

            StatusMessage = sessionDescription;
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles =Userroles.Divepro)]
        public async Task<IActionResult> ApplySingle(int sessionId)
        {
            string sessionDescription = "Du har skrevet dig på som instruktør til: \n";

            var session = await db.CourseSessions
                    .Include(x => x.Course)
                    .Include(x => x.CourseSessionTemplate)
                    .Include(x => x.SessionInstructors)
                    .FirstOrDefaultAsync(x => x.Id == sessionId);

            ActiveCourse = session.Course.Id;

            session.SessionInstructors.Add(new CourseSessionInstructor() { InstructorId = User.GetIdentityId() });
            await db.SaveChangesAsync();

            var sessionText = session.CourseSessionTemplate != null ? session.CourseSessionTemplate.Name : session.SessionName;
            sessionDescription += $"{sessionText} d. {session.DateTime.ToString("dd. MMMM yyyy")} \n";

            StatusMessage = sessionDescription;
            return RedirectToAction(nameof(Workcalendar));
        }

        [Authorize(Roles = Userroles.Divepro)]
        public async Task<IActionResult> Retract(int sessionid)
        {
            var userId = User.GetIdentityId();
            var user = await db.ApplicationUsers.FindAsync(userId);

            var sessionInstructor = await db.CourseSessionInstructors
                .Include(x=>x.CourseSession).ThenInclude(x=>x.Course)
                .Include(x => x.CourseSession).ThenInclude(x => x.CourseSessionTemplate)
                .Where(x => x.InstructorId == userId && x.CourseSessionId == sessionid).FirstOrDefaultAsync();
            sessionInstructor.InstructorRetracted = true;

            var sessionText = $"'{sessionInstructor.CourseSession.SessionDescription}'";
            var sessionDescription = $"Du har afmeldt dig til {sessionText} d. {sessionInstructor.CourseSession.DateTime.ToString("dd. MMMM yyyy")}.\n";

            await db.SaveChangesAsync();

            StatusMessage = sessionDescription;

            var admins = await userManager.GetUsersInRoleAsync("Administrator");
           
            await smsSender.SendMultipleSmsAsync(admins, $"{user.Name } har meldt fra som instruktør til {sessionText} d. {sessionInstructor.CourseSession.DateTime.ToString("dd. MMMM yyyy")}.\n");

            return RedirectToAction(nameof(Workcalendar));

        }


        [Authorize(Roles = Userroles.Administrator + ", " + Userroles.Owner)]
        public async Task<IActionResult> Approve(int sessionid, string instructorId)
        {
            var sessioninstructor = await db.CourseSessionInstructors.FindAsync(sessionid, instructorId);
            var instructor = await db.ApplicationUsers.FindAsync(instructorId);
            var session = await db.CourseSessions.Include(x => x.CourseSessionTemplate).Include(x=>x.Course).FirstOrDefaultAsync(x => x.Id == sessionid);
            sessioninstructor.InstructorApproved = !sessioninstructor.InstructorApproved;
            ActiveCourse = session.Course.Id;

            await db.SaveChangesAsync();

            if (instructor.PhoneNumberConfirmed)
            {
                switch (sessioninstructor.InstructorApproved)
                {
                    case true:
                        await smsSender.SendSmsAsync(instructor.PhoneNumber, $"Du har undervisning på kurset {session.SessionName} d. {session.DateTime}. Du kan se din fulde liste over dine kursusdage på klub-sitet under din profil - Min undervisningplan");
                        break;
                    case false:
                        await smsSender.SendSmsAsync(instructor.PhoneNumber, $"Du er blevet fjernet som instruktør på kurset \n {session.SessionName}\n d. {session.DateTime}.\n Hvis dette er en fejl, bedes du kontakte Scansub DK Diver\n  Du kan se din fulde liste over dine kursusdage på klub-sitet under din profil - Min undervisningplan");
                        break;
                }
            }
            else if (instructor.PhoneNumberConfirmed)
            {
                switch (sessioninstructor.InstructorApproved)
                {
                    case true:
                        StatusMessage = $"Du har aktiveret {instructor.Name} som instruktør på kurset {session.SessionName} d. {session.DateTime}, men vedkommende har ikke fået en SMS - Husk at give vedkommende besked";
                        break;
                    case false:
                        StatusMessage = $"Du har deaktiveret {instructor.Name} som instruktør på kurset {session.SessionName} d. {session.DateTime}, men vedkommende har ikke fået en SMS - Husk at give vedkommende besked";
                        break;
                }
            }

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = Userroles.Divepro)]
        public async Task<IActionResult> MyWorkplan()
        {
            var workplan = await db.CourseSessionInstructors
                .Include(x=>x.CourseSession).ThenInclude(x=>x.Course)
                .Include(x => x.CourseSession).ThenInclude(x => x.CourseSessionTemplate)
                .Where(x => x.InstructorId == User.GetIdentityId() && x.CourseSession.DateTime > DateTime.Now)
                .ToListAsync();

            return View(workplan);

        }
    }
}