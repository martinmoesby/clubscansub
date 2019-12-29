using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> um;
        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> um)
            : base(db)
        {
            this.um = um;
        }

        public async Task<IActionResult> Index()
        {

            //Get Members only
            //var m_usrs = await um.GetUsersInRoleAsync(Userroles.Member);
            //var p_users = await um.GetUsersInRoleAsync(Userroles.Divepro);
            //var a_users = await um.GetUsersInRoleAsync(Userroles.Administrator);
            //var owner = await um.GetUsersInRoleAsync(Userroles.Owner);

            var currentUser = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            var members = await db.ApplicationUsers.OrderBy(x=>x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id }).ToListAsync();

            var locations = await db.Divelocations.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = false }).ToListAsync();
            var certificates = await db.Certificates.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = false }).ToListAsync();
            var coursetemplates = await db.CourseTemplates.Where(x => x.Sessions.Count > 0).Select(x => new SelectListItem() { Text = x.TemplateName, Value = x.Id.ToString(), Selected = false }).ToListAsync();

            var pageModel = new HomeIndexViewModel()
            {
                NewEvent = new CreateEventViewModel(),
                NewCourse = new CreateCourseViewModel(),
                NewTransaction = new CreateUserAccountTransactionViewModel(),
                AppUser = currentUser
            };

            pageModel.NewTransaction.Entry = new ApplicationUserAccountEntry() { PostingDate = new DateTime() };
            pageModel.NewEvent.Event = new Event()
            {
                StartDateAndTime = DateTime.Now.Date.AddDays(1).AddHours(9),
                EndDateAndTime = DateTime.Now.Date.AddDays(1).AddHours(14)
            };

            pageModel.NewCourse.Coursetemplates = coursetemplates;
            pageModel.NewEvent.Locations = locations;
            pageModel.NewEvent.Certificates = certificates;
            pageModel.NewTransaction.Members = members;

            return View(pageModel);
        }

        public IActionResult Planner(DateTime? ActiveDate)
        {
            if (!ActiveDate.HasValue)
                ActiveDate = DateTime.Now;

            PlannerViewModel plannerVM = new PlannerViewModel()
            {
                Courses = db.CourseTemplates.Where(x => x.Sessions.Count > 0).ToList(),
                Divesites = db.Divelocations.ToList(),
                ActiveDate = ActiveDate.GetValueOrDefault()
            };

            return View(plannerVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, DateTime dateStr)
        {
            var parms = id.Split(":");
            var eventType = parms[0];
            var eventId = int.Parse(parms[1]);

            if (eventType == "eventId")
            {

                var @event = await db.Events
                    .Include(x => x.Divelocation)
                    .FirstOrDefaultAsync(x => x.Id == eventId);
                return PartialView("_EditEventFromPlannerPartial",@event);
            }
            else 
            {
                var courseSession = await db.CourseSessions.Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id == eventId);
                var course = courseSession.Course;

                return PartialView("_EditCourseFromPlannerPartial", course);

            }
        }

        [HttpPost,ActionName("EditEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event entity)
        {
            db.Attach(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();
            
            return RedirectToAction(nameof(Planner), new { ActiveDate = entity.StartDateAndTime });

        }

        [HttpPost, ActionName("EditCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            db.Attach(course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Planner), new { ActiveDate = course.StartDateAndTime });

        }

        public IActionResult Create(int id, string type, DateTime startdate)
        {
            if (type == "trip")
                CreateEvent(id, startdate);
            else
                CreateCourse(id, startdate);

            return RedirectToAction(nameof(Planner), new { ActiveDate = startdate });
        }

        public async Task<IActionResult> Move(string id, DateTime startdate)
        {
            var idItems = id.Split(":");
            int itemId = int.Parse(idItems[1]);
            string itemType = idItems[0];

            var newDate = startdate.Date;
            if (itemType == "eventId")
            {
                var item = await db.Events.FindAsync(itemId);

                item.StartDateAndTime = newDate.Add(item.StartDateAndTime.TimeOfDay);
                item.EndDateAndTime = newDate.Add(item.EndDateAndTime.TimeOfDay);
            }
            else
            {
                var item = await db.CourseSessions.FindAsync(itemId);
                var beginTime = item.DateTime.TimeOfDay;
                item.DateTime = startdate.Add(beginTime);
            }
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Planner), new { ActiveDate = startdate });

        }

        private void CreateEvent(int divelocationId, DateTime eventdate)
        {
            var location = db.Divelocations.Include(x => x.Certificate).Include(x=>x.MeetingLocation).FirstOrDefault(x => x.Id == divelocationId);
            var certificate = location.Certificate;

            var item = new Event();
            item.EventType = location.DefaultEventType;

            item.StartDateAndTime = eventdate.Add(location.DefaultStartTime);
            item.EndDateAndTime = item.StartDateAndTime.Add(location.DefaultDuration);
            item.Address = location.MeetingLocation;

            if (item.Price == 0)
                item.Price = location.Price;

            item.Divelocation = location;
            item.RequiredCertificate = certificate;

            if (string.IsNullOrEmpty(item.Title))
            {
                switch (item.EventType)
                {
                    case EventTypeEnum.Bådtur:
                        item.Title = $"Bådtur til {location.Name}.";
                        item.MaxParticipants = 10;
                        break;
                    case EventTypeEnum.Stranddyk:
                        item.Title = $"Strandtur til {location.Name}.";
                        item.MaxParticipants = 50;
                        break;
                    case EventTypeEnum.Rejse:
                        item.Title = $"Rejse til {location.Name}.";
                        item.MaxParticipants = 12;
                        break;
                    case EventTypeEnum.Liveaboard:
                        item.Title = $"Liveaboard tur til {location.Name}";
                        item.MaxParticipants = 20;
                        break;
                    case EventTypeEnum.Klubture:
                    default:
                        item.Title = location.Name;
                        break;
                }
            }

            item.Details += $"{item.Title.TrimEnd('.')} - kræver mindst { item.MinParticipants} deltagere og der er plads til maksimalt { item.MaxParticipants}";

            db.Events.Add(item);

            db.SaveChanges();


        }

        private void CreateCourse(int templateId, DateTime startdate)
        {
            var template = db.CourseTemplates
                .Include(x => x.Sessions)
                .ThenInclude(x=>x.Address)
                .FirstOrDefault(x => x.Id == templateId);

            var course = new Course()
            {
                CourseName = template.TemplateName,
                CourseTemplate = template,
                CourseType = template.CourseType,
                MinParticipants = template.MinStudents,
                MaxParticipants = template.MaxStudents,
                StartDateAndTime = startdate,
                Price = template.DefaultPrice,
                EventType = EventTypeEnum.NotAnEvent
            };

            var currentDate = course.StartDateAndTime;
            var currentWeekDay = course.StartDateAndTime.Date.DayOfWeek;
            if (template.Sessions.Count > 0)
            {
                course.CourseSessions = new List<CourseSession>();

                foreach (var item in template.Sessions.OrderBy(x => x.SessionNumber))
                {
                    if (item.UseDefaultWeekDay)
                    {
                        while (currentWeekDay != item.DefaultWeekday)
                        {
                            currentDate = currentDate.AddDays(1);
                            currentWeekDay = currentDate.DayOfWeek;
                        }
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
            db.SaveChanges();

        }

    }
}