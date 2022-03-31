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
using Microsoft.AspNetCore.Mvc.Rendering;
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
            _PageModel = new PageModel();
            _NewSessionPageModel = new NewSessionPageModel();
        }
        public class PageModel
        {
            public Course Course { get; set; }
            public IEnumerable<SelectListItem> UsersList { get; set; }
            public IEnumerable<SelectListItem> DiveLocations { get; set; }

        }

        public class NewSessionPageModel
        {
            public int CourseId { get; set; }
            public int SelectedAddressId { get; set; }
            public int SelectedDivesiteId { get; set; }

            public CourseSession Session { get; set; }
            public List<SelectListItem> DiveLocations { get; set; }
            public List<SelectListItem> Addresses { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public PageModel _PageModel { get; set; }

        [BindProperty]
        public NewSessionPageModel _NewSessionPageModel { get; set; }

        public IActionResult Index(CourseTypeEnum courseType = CourseTypeEnum.BaseCourse)
        {
            ViewBag.Pagetitle = "Kommende eller igangværende ";

            switch (courseType)
            {
                case CourseTypeEnum.BaseCourse:
                    ViewBag.Pagetitle += "Basis kurser";
                    break;
                case CourseTypeEnum.TechCourse:
                    ViewBag.Pagetitle += "Tekniske kurser";
                    break;
                case CourseTypeEnum.ProCourse:
                    ViewBag.Pagetitle += "Pro Kurser";
                    break;
                case CourseTypeEnum.SpecialtyCourse:
                    ViewBag.Pagetitle += "Specialer";
                    break;
                default:
                    ViewBag.Pagetitle += "Andre kurser";
                    break;
            }

            ViewBag.StatusMessage = StatusMessage;
            ViewBag.IsCompleteCourses = false;
            ViewBag.CourseType = courseType;
            return View(db.Courses.Where(x=>x.CourseType == courseType && x.EndDateAndTime > DateTime.Now).Include(x=>x.CourseSessions).Include(x=>x.Participants).Include(x=>x.Signups));

        }

        public IActionResult CompletedCourseIndex(CourseTypeEnum courseType = CourseTypeEnum.BaseCourse)
        {
            ViewBag.Pagetitle = "Afsluttede ";

            switch (courseType)
            {
                case CourseTypeEnum.BaseCourse:
                    ViewBag.Pagetitle += "Basis kurser";
                    break;
                case CourseTypeEnum.TechCourse:
                    ViewBag.Pagetitle += "Tekniske kurser";
                    break;
                case CourseTypeEnum.ProCourse:
                    ViewBag.Pagetitle += "Pro Kurser";
                    break;
                case CourseTypeEnum.SpecialtyCourse:
                    ViewBag.Pagetitle += "Specialer";
                    break;
                default:
                    ViewBag.Pagetitle += "Andre kurser";
                    break;
            }

            ViewBag.StatusMessage = StatusMessage;
            ViewBag.IsCompleteCourses = true;
            ViewBag.CourseType = courseType;
            return View("Index", db.Courses.Where(x => x.CourseType == courseType && x.EndDateAndTime <= DateTime.Now).Include(x => x.CourseSessions).Include(x => x.Participants).Include(x => x.Signups));

        }

        public async Task<IActionResult> Edit(int id)
        {
            _PageModel.Course = await db.Courses
                .Include(x => x.CourseSessions)
                    .ThenInclude(x=>x.CourseSessionTemplate)
                .Include(x=>x.CourseSessions)
                    .ThenInclude(x=>x.SessionInstructors)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.Address)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.Divelocation)
                        .ThenInclude(x=>x.MeetingLocation)
                .Include(x=>x.Signups)
                    .ThenInclude(c=>c.ApplicationUser)
                .Include(x=>x.Participants)
                    .ThenInclude(c=>c.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            _PageModel.UsersList = await db.ApplicationUsers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).Select(x => new SelectListItem()
            {
                Text = $"{x.Name} ({x.AccountNumber})",
                Value = x.Id
            }).ToListAsync();

            _PageModel.DiveLocations = await db.Divelocations.OrderBy(x => x.Name).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            ViewBag.StatusMessage = StatusMessage;

            return View(_PageModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse()
        {
            db.Attach(_PageModel.Course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _PageModel.Course.EventType = EventTypeEnum.NotAnEvent;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { courseType = _PageModel.Course.CourseType });
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
                EventType = EventTypeEnum.NotAnEvent,
                DeeplinkId = Guid.NewGuid()
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


        public async Task<IActionResult> GetSession ( int Id, bool isDelete)
        {
            var session = await db.CourseSessions
                .Include(x=>x.Course).ThenInclude(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Include(x=>x.CourseSessionTemplate)
                .Include(x=>x.SessionInstructors).ThenInclude(x=>x.Instructor)
                .Include(x=>x.Address)
                .Include(x=>x.Divelocation)
                .FirstOrDefaultAsync(x=>x.Id == Id);

            if (string.IsNullOrEmpty(session.SessionName)) session.SessionName = session.CourseSessionTemplate?.Name;
            if (string.IsNullOrEmpty(session.SessionDescription)) session.SessionDescription = session.CourseSessionTemplate?.Description;

            var partialView = isDelete ? "_DeleteSessionPartial" : "_EditSessionPartial";
            return PartialView(partialView, session);
        }

        public async Task<IActionResult> AddSession(int id)
        {
            _NewSessionPageModel.CourseId = id;
            _NewSessionPageModel.SelectedDivesiteId = 0;
            _NewSessionPageModel.SelectedAddressId = 0;
            _NewSessionPageModel.Session = new CourseSession()
            {
                DateTime = DateTime.Now.AddDays(1).Date.AddHours(8),
                Duration = new TimeSpan(4,0,0),
                Divelocation = null,
                Address = null,
            };
            _NewSessionPageModel.DiveLocations = await db.Divelocations.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
            _NewSessionPageModel.Addresses = await db.Divelocations.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
            
            
            return PartialView("_AddSessionPartial", _NewSessionPageModel);
        }
        [HttpPost, ActionName("AddSession")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSession()
        {
            _NewSessionPageModel.Session.Course = await db.Courses.FindAsync(_NewSessionPageModel.CourseId); 
            _NewSessionPageModel.Session.Address = await db.Addresses.FindAsync(_NewSessionPageModel.SelectedAddressId);
            _NewSessionPageModel.Session.Divelocation = await db.Divelocations.FindAsync(_NewSessionPageModel.SelectedDivesiteId);

            await db.CourseSessions.AddAsync(_NewSessionPageModel.Session);
            await db.SaveChangesAsync();

            var course = await db.Courses.FindAsync(_NewSessionPageModel.CourseId);

            var maxEndDate = await db.CourseSessions.Where(x=>x.Course == course).MaxAsync(x => x.DateTime.Add(x.Duration));
            var minEndDate = await db.CourseSessions.Where(x => x.Course == course).MinAsync(x => x.DateTime);
            course.StartDateAndTime = minEndDate;
            course.EndDateAndTime = maxEndDate;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = _NewSessionPageModel.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSession(CourseSession session)
        {
            //TODO: Update database with new session details 
            var existingSession = await db.CourseSessions
                .Include(x=>x.Course).ThenInclude(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.SessionInstructors).ThenInclude(x => x.Instructor)
                .Include(x => x.Address)
                .Include(x => x.Divelocation)
                .FirstOrDefaultAsync(x => x.Id == session.Id);

            if (existingSession.DateTime != session.DateTime)
            {
                var smsMessage = $"Kursusdagen for kurset {existingSession.Course.CourseName} d. {existingSession.DateTime.ToLongDateString()} kl. {existingSession.DateTime.ToShortTimeString()} er blevet flyttet til  d. {session.DateTime.ToLongDateString()} kl. {session.DateTime.ToShortTimeString()}. Kontakt DK Diver for yderligere information";
                existingSession.DateTime = session.DateTime;

                if (existingSession.Course.Participants?.Count() > 0)
                {
                    var notifyresult = await smsSender.SendMultipleSmsAsync(existingSession.Course.Participants.Select(x => x.ApplicationUser), smsMessage);     //TODO: Notify students and instructors about the change

                    foreach (var item in notifyresult)
                    {
                        StatusMessage += item.GetErrorInfo();
                    }
                }

                if (existingSession.SessionInstructors?.Count() > 0)
                {
                    var notifyresult = await smsSender.SendMultipleSmsAsync(existingSession.Course.Participants.Select(x => x.ApplicationUser), smsMessage);     //TODO: Notify students and instructors about the change

                    foreach (var item in notifyresult)
                    {
                        StatusMessage += item.GetErrorInfo();
                    }

                }
            }

            existingSession.Duration = session.Duration;
            existingSession.SessionDescription = session.SessionDescription;
            existingSession.SessionName = session.SessionName;

            //if (existingSession.Divelocation?.Id != session.Divelocation.Id)
            //{
            //    existingSession.Divelocation.Id = session.Divelocation.Id;
            //    // TODO: Notify students and instructors
            //}
            //if (existingSession.Address?.Id != session.Address.Id)
            //{
            //    existingSession.Address.Id = session.Address.Id;
            //    // TODO: Notify students and instructors
            //}

            await db.SaveChangesAsync();

            var course = await db.Courses.FindAsync(session.Course.Id);

            var maxEndDate = await db.CourseSessions.Where(x => x.Course == course).MaxAsync(x => x.DateTime.Add(x.Duration));
            var minEndDate = await db.CourseSessions.Where(x => x.Course == course).MinAsync(x => x.DateTime);
            course.StartDateAndTime = minEndDate;
            course.EndDateAndTime = maxEndDate;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = session.Course.Id });
        }

        [HttpPost,ActionName("DeleteSession")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSession(CourseSession session)
        {
            //TODO: Get the session and instructors and participants to be able to notify of the changed schedule
            var existingSession = await db.CourseSessions
                .Include(x=>x.Course).ThenInclude(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.SessionInstructors).ThenInclude(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.Id == session.Id);

            var smsMessage = $"Kursusdagen for kurset {existingSession.Course.CourseName} d. {existingSession.DateTime.ToLongDateString()} kl. {existingSession.DateTime.ToShortTimeString()} er blevet aflyst. Kontakt DK Diver for yderligere information";

            if (existingSession.Course.Participants?.Count() > 0)
            {
                var notifyresult = await smsSender.SendMultipleSmsAsync(existingSession.Course.Participants.Select(x => x.ApplicationUser), smsMessage);     //TODO: Notify students and instructors about the change

                foreach (var item in notifyresult)
                {
                    StatusMessage += item.GetErrorInfo();
                }
            }

            if (existingSession.SessionInstructors?.Count() >0 )
            {
                var notifyresult = await smsSender.SendMultipleSmsAsync(existingSession.Course.Participants.Select(x => x.ApplicationUser), smsMessage);     //TODO: Notify students and instructors about the change

                foreach (var item in notifyresult)
                {
                    StatusMessage += item.GetErrorInfo();
                }

            }
            db.CourseSessions.Remove(existingSession);
            await db.SaveChangesAsync();

            var course = await db.Courses.FindAsync(session.Course.Id);

            var maxEndDate = await db.CourseSessions.Where(x => x.Course == course).MaxAsync(x => x.DateTime.Add(x.Duration));
            var minEndDate = await db.CourseSessions.Where(x => x.Course == course).MinAsync(x => x.DateTime);
            course.StartDateAndTime = minEndDate;

            course.EndDateAndTime = maxEndDate;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = existingSession.Course.Id });
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

            if (applicationUser.PhoneNumberConfirmed)
            {
                await smsSender.SendSmsAsync(applicationUser.PhoneNumber, $"Du er blevet afmeldt kurset '{course.CourseName}' der begynder den {course.StartDateAndTime}, og beløbet er refunderet til din kursus-konto til brug for et senere tidspunkt");
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        public async Task<IActionResult> AddUserToCourse(int courseId, string userId, bool doCreateAccountTransaction = false)
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

            if (doCreateAccountTransaction)
            {
                db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                {
                    AccountType = AccountTypeEnum.CourseAccountType,
                    Amount = -course.Price,
                    Description = $"Betaling for Kursus {course.CourseName}",
                    PostingDate = DateTime.Now,
                    Event = course,
                    ApplicationUser = applicationUser
                });
            }

            if (applicationUser.PhoneNumberConfirmed)
            {
                var smsMessage = $"Hej {applicationUser.Firstname},\n\n" +
                    $"Du er nu blevet tilmeldt kurset \n\n{course.CourseName}\n\n med start \n\nd. {course.StartDateAndTime}.\n" +
                    $"Dit kursus materiale er tilgængeligt på www.divessi.com og i DiveSSI-appen\n" +
                    $"Det er en fordel, hvis du har gennemgået materialet inden den 1. kursusaften begynder.\n" +
                    $"\n" +
                    $"Vi glæder os rgtig meget til at se dig.\n" +
                    $"\n" +
                    $"Med venlig hilsen\n" +
                    $"Scansub DK Diver";

                await smsSender.SendSmsAsync(applicationUser.PhoneNumber, smsMessage);
                StatusMessage = $"{applicationUser.Firstname} er blevet tilmeldt og har fået beksed via SMS";
            }

            if (applicationUser.EmailConfirmed)
            {
                var mailMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du er nu blevet tilmeldt kurset {course.CourseName} med start d. {course.StartDateAndTime}.<br />" +
                    $"Dit kursus materiale er tilgængeligt på www.divessi.com og i DiveSSI appen <br/>" +
                    $"Det er en fordel, hvis du har gennemgået materialet inden den 1. kursusaften begynder. <br />" +
                    $"<br />" +
                    $"Vi glæder os rgtig meget til at se dig.<br/>" +
                    $"<br />" +
                    $"<br/><br/>Med venlig hilsen <br/><br/> Scansub DK Diver";

                await emailSender.SendEmailAsync(applicationUser.Email, $"Tilmelding til {course.CourseName}",mailMessage );
                StatusMessage = $"{applicationUser.Firstname} er blevet tilmeldt og har fået besked vial mail";
            }

            if (!applicationUser.EmailConfirmed && !applicationUser.PhoneNumberConfirmed)
            {
                StatusMessage = $"Fejl: {applicationUser.Firstname} har hverken valideret sin email eller sit telefonr. så det har ikke været muligt at give elektronisk besked.";
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSessionLocation(int CourseId, int SessionId, int LocationId)
        {
            var session = await db.CourseSessions.FindAsync(SessionId);
            var location = await db.Divelocations.Include(x=>x.MeetingLocation).FirstOrDefaultAsync(x=>x.Id == LocationId);

            session.Divelocation = location;

            await db.SaveChangesAsync();

            var users = await db.Courses.Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == CourseId);
            var instructors = await db.CourseSessions.Include(x => x.SessionInstructors).ThenInclude(x=>x.Instructor).FirstOrDefaultAsync(x => x.Id == SessionId);
            var notifyusers = users.Participants.Select(x=>x.ApplicationUser).ToList();
            notifyusers.AddRange(instructors.SessionInstructors.Select(x => x.Instructor));

            var userSmsString = $"Skift af Dykkersted.\n\nMødestedet for kursusdagen d. {session.DateTime} er blevet skiftet til \n\n" +
                $"\"{location.Name}\" \n\n" +
                $"Vi skal nu mødes: \n" +
                $"{location.MeetingLocation.Name}\n" +
                $"{location.MeetingLocation.Streetname}\n" +
                $"{location.MeetingLocation.Zipcode} {location.MeetingLocation.City}\n\n" +
                $"Du kan finde en Rutebeskrivelse under 'Mine kurser' på kursuskalenderen\n\n" +
                $"kalender.klubscansub.dk \n\n" +
                $"Med venlig hilsen\n" +
                $"Dk Diver";

            var responses = await smsSender.SendMultipleSmsAsync(notifyusers, userSmsString);

            if (responses.Any(x => x.IsSuccessStatusCode == false))
            {
                StatusMessage = "FEJL:" + string.Join(',', responses.Where(x => x.IsSuccessStatusCode == false).Select(x => x.GetErrorMessage()));
            } else
            {
                StatusMessage = "Alle brugere og instruktører er har fået beksed via SMS";
            }

            return RedirectToAction(nameof(Edit), new { id = CourseId });

        }

        //public async Task<JsonResult> FindUsersAsyn(string searchstring)
        //{

        //}
    }
}