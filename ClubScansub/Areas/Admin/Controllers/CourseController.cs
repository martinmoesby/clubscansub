using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : BaseAdminController
    {
        private readonly ISmsSender smsSender;
        private readonly IEmailSender emailSender;

        public CourseController(ApplicationDbContext context, ISmsSender smsSender, IEmailSender emailSender)
            :base(context)
        {
            this.smsSender = smsSender;
            this.emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        public IActionResult Index(CourseTypeEnum courseType = CourseTypeEnum.BaseCourse)
        {

            switch (courseType)
            {
                case CourseTypeEnum.BaseCourse:
                    ViewBag.Pagetitle = "Basis kurser";
                    break;
                case CourseTypeEnum.TechCourse:
                    ViewBag.Pagetitle = "Tekniske kurser";
                    break;
                case CourseTypeEnum.ProCourse:
                    ViewBag.Pagetitle = "Pro Kurser";
                    break;
                case CourseTypeEnum.SpecialtyCourse:
                    ViewBag.Pagetitle = "Specialer";
                    break;
                default:
                    ViewBag.Pagetitle = "Andre kurser";
                    break;
            }

            ViewBag.StatusMessage = StatusMessage;

            return View(db.Courses.Where(x=>x.CourseType == courseType).Include(x=>x.CourseSessions).Include(x=>x.Participants).Include(x=>x.Signups));

        }

        public async Task<IActionResult> Edit(int id)
        {
            Course = await db.Courses
                .Include(x => x.CourseSessions)
                    .ThenInclude(x=>x.CourseSessionTemplate)
                .Include(x=>x.CourseSessions)
                    .ThenInclude(x=>x.SessionInstructors)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.Address)
                .Include(x=>x.Signups)
                    .ThenInclude(c=>c.ApplicationUser)
                .Include(x=>x.Participants)
                    .ThenInclude(c=>c.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            return View(Course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse()
        {
            db.Attach(Course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            Course.EventType = EventTypeEnum.NotAnEvent;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { courseType = Course.CourseType });
        }

        [HttpPost, ActionName("CreateCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel courseviewmodel)
        {
            var template = await db.CourseTemplates
                .Include(x=>x.Sessions)
                .ThenInclude(x=>x.Address)
                .FirstOrDefaultAsync(x=>x.Id == courseviewmodel.CourseTemplateId);

            var course = new Course()
            {
                CourseName = courseviewmodel.Course.CourseName,
                CourseTemplate = template,
                CourseType = template.CourseType,
                MinParticipants = courseviewmodel.Course.MinParticipants,
                MaxParticipants = courseviewmodel.Course.MaxParticipants,
                StartDateAndTime = courseviewmodel.Course.StartDateAndTime,
                Price = courseviewmodel.Course.Price,
                EventType = EventTypeEnum.NotAnEvent
            };

            var currentDate = course.StartDateAndTime;
            var currentWeekDay = course.StartDateAndTime.Date.DayOfWeek;
            if (template.Sessions.Count > 0)
            {
                course.CourseSessions = new List<CourseSession>();

                foreach (var item in template.Sessions.OrderBy(x => x.SessionNumber))
                {

                    while (currentWeekDay != item.DefaultWeekday)
                    {
                        currentDate = currentDate.AddDays(1);
                        currentWeekDay = currentDate.DayOfWeek;
                    }

                    course.CourseSessions.Add(new CourseSession()
                    {
                        CourseSessionTemplate = item,
                        DateTime = currentDate.Add(item.DefaultStartTime),
                        Sessiontype = item.SessionType,
                        Duration = item.DefaultDuration,
                        Address = item.Address
                    });

                    currentDate = currentDate.AddDays(1);
                    currentWeekDay = currentDate.DayOfWeek;
                }
            }

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { courseType = course.CourseType });
        }

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await db.Courses
                .Include(x => x.Participants)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Signups)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.SessionInstructors)
                        .ThenInclude(x => x.Instructor)
                .Include(x=>x.CourseSessions)
                    .ThenInclude(x=> x.CourseSessionTemplate)
                .FirstOrDefaultAsync(x => x.Id == id);

            var courseType = course.CourseType;

            return View(course);
    
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await db.Courses
                .Include(x => x.Participants)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Signups)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.SessionInstructors)
                        .ThenInclude(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.Id == id);

            var courseType = course.CourseType;

            // Notify Participants

            var participantsResult = await smsSender.SendMultipleSmsAsync(course.Participants.Select(x => x.ApplicationUser), $"Kurset {course.CourseName} der skulle starte d. {course.StartDateAndTime} er desværre blevet aflyst. Kursus prisen er blevet refunderet til din Kursuskonto til senere brug.");
            var signupResult = await smsSender.SendMultipleSmsAsync(course.Signups.Select(x => x.ApplicationUser), $"Kurset {course.CourseName} der skulle starte d. {course.StartDateAndTime} er desværre blevet aflyst.");

            var instructors = new List<ApplicationUser>();
            foreach (var session in course.CourseSessions)
            {
                foreach (var instructor in session.SessionInstructors)
                {
                    instructors.Add(instructor.Instructor);
                }
            }

            var instructorResult = await smsSender.SendMultipleSmsAsync(instructors.Distinct(), $"Kurset {course.CourseName} der skulle starte d. {course.StartDateAndTime} er desværre blevet aflyst.");

            foreach (var item in participantsResult)
            {
                StatusMessage += item.GetErrorMessage();
            }

            foreach (var item in signupResult)
            {
                StatusMessage += item.GetErrorMessage();
            }

            foreach (var item in instructorResult)
            {
                StatusMessage += item.GetErrorMessage();
            }

            db.Courses.Remove(course);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { courseType });
        }

        [HttpGet]
        public async Task<JsonResult> GetCoursePrice(int templateid)
        {
            var template = await db.CourseTemplates.FindAsync(templateid);
            return new JsonResult(template.DefaultPrice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSession(int courseId, CourseSession session)
        {
            db.Attach(session).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        public async Task<IActionResult> RemoveUserFromCourse(int courseId, string userId)
        {
            // TODO - remove user from Course and return payment to users CourseAccountBalance

            var course = await db.Courses.Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == courseId);
            var applicationUser = await db.ApplicationUsers.FindAsync(userId);
            var eventUser = course.Participants.FirstOrDefault(x => x.ApplicationUserId == userId);

            course.Participants.Remove(eventUser);

            // Validate that the full amount should be refunded
            db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
            {
                AccountType = AccountTypeEnum.CourseAccountType,
                Amount = course.Price,
                Description = $"Refund - Course {course.CourseName}",
                PostingDate = DateTime.Now,
                Event = course,
                ApplicationUser = applicationUser
            });

            await smsSender.SendSmsAsync(applicationUser.PhoneNumber, $"Du er blevet afmeldt kurset '{course.CourseName}' der begynder den {course.StartDateAndTime}, og beløbet er runderet til din kursus-konto til brug for et senere tidspunkt");

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        public async Task<IActionResult> AddUserToCourse(int courseId, string userId)
        {
            // TODO - add user to Course and withdraw payment from users CourseAccountBalance
            // TODO - remove user from SignUps

            var course = await db.Courses
                .Include(x=>x.Signups)
                .Include(x=>x.Participants)
                .FirstOrDefaultAsync(x => x.Id == courseId);

            var applicationUser = await db.ApplicationUsers.FindAsync(userId);

            var courseSignup = course.Signups.FirstOrDefault(x => x.ApplicationUserId == userId);
            course.Signups.Remove(courseSignup);
            course.Participants.Add(new EventUser() { ApplicationUserId = userId });

            db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
            {
                AccountType = AccountTypeEnum.CourseAccountType,
                Amount = -course.Price,
                Description = $"Betaling for Kursus {course.CourseName}",
                PostingDate = DateTime.Now,
                Event  = course,
                ApplicationUser = applicationUser
            });
            
            if (applicationUser.PhoneNumberConfirmed)
            {
                await smsSender.SendSmsAsync(applicationUser.PhoneNumber, $"Du er blevet tilmeldt kurset '{course.CourseName}' der begynder den {course.StartDateAndTime} - vi glæder os til at se dig");
            } else
            {
                await emailSender.SendEmailAsync(applicationUser.Email, $"Tilmelding til {course.CourseName}", $"Du er blevet tilmeldt kurset {course.CourseName} med start d. {course.StartDateAndTime}");
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }
        
    }
}