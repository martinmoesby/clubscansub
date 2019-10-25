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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Divepro.Controllers
{
    [Area("Divepro")]
    public class WorkController : BaseDiveproController
    {
        private ISmsSender smsSender;

        public WorkController(ApplicationDbContext db, ISmsSender smsSender)
            : base(db)
        {
            this.smsSender = smsSender;
        }
        
        [TempData]
        public string StatusMessage { get; set; }
        
        public async Task<IActionResult> Index()
        {
            ViewBag.Pagetitle = "Kurser der mangler instruktøer";
            ViewBag.StatusMessage = StatusMessage;

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
                .Where(x => x.StartDateAndTime > DateTime.Now)
                .OrderBy(x=>x.StartDateAndTime)
                .ToListAsync();

            return View(courses);
        }

        [Authorize(Roles = Userroles.Divepro)]
        public async Task<IActionResult> Apply(int sessionid)
        {
            var session = await db.CourseSessions
                .Include(x => x.CourseSessionTemplate)
                .Include(x => x.SessionInstructors)
                .FirstOrDefaultAsync(x => x.Id == sessionid);

            session.SessionInstructors.Add(new CourseSessionInstructor() { InstructorId = User.GetIdentityId() });

            //var sessioninstructor = new CourseSessionInstructor()
            //{
            //    CourseSessionId = sessionid,
            //    InstructorId = User.GetIdentityId()
            //};

            //db.CourseSessionInstructors.Add(sessioninstructor);
            await db.SaveChangesAsync();

            StatusMessage = $"Du har skrevet dig på som instruktør til {session.CourseSessionTemplate.Name} d. {session.DateTime.ToString("dd. MMMM yyyy")}";

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = Userroles.Administrator + ", " + Userroles.Owner)]
        public async Task<IActionResult> Approve(int sessionid, string instructorId)
        {
            var sessioninstructor = await db.CourseSessionInstructors.FindAsync(sessionid, instructorId);
            var instructor = await db.ApplicationUsers.FindAsync(instructorId);

            var session = await db.CourseSessions.Include(x => x.CourseSessionTemplate).FirstOrDefaultAsync(x => x.Id == sessionid);

            sessioninstructor.InstructorApproved = true;

            if (instructor.PhoneNumberConfirmed)
            {
                await smsSender.SendSmsAsync(instructor.PhoneNumber, $"Du har undervisning på kurset {session.CourseSessionTemplate.Name} d. {session.DateTime}. Du kan se din fulde liste over dine kursusdage på klub-sitet under din profil - Min undervisningplan");
            }
            else
            {
                StatusMessage = $"Du har aktiveret {instructor.Name} som instruktør på kurset {session.CourseSessionTemplate.Name} d. {session.DateTime}, men vedkommende har ikke fået en SMS- Husk at give vedkommende besked";
            }

            await db.SaveChangesAsync();

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