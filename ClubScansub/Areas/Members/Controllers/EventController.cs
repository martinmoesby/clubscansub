using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Members.Controllers
{
    [Area("Members")]
    [Authorize(Roles = Userroles.Member)]
    public class EventController : BaseController
    {
        public EventController(ApplicationDbContext db)
            :base(db)
        {

        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IEnumerable<Event> Events { get; set; }

        [BindProperty]
        public IEnumerable<Course> Courses { get; set; }


        public async Task<IActionResult> Index()
        {
            Events = await db.EventUsers.Include(x=>x.Event)
                .Where(x=> !x.Event.IsCancelled && x.ApplicationUserId == User.GetIdentityId() && x.Event.EventType != EventTypeEnum.NotAnEvent)
                .Select(x=>x.Event).Include(x=>x.Participants)
                .OrderBy(x=>x.StartDateAndTime).ThenBy(x=>x.Id)
                .ToListAsync();

            return View(Events);
        }

        public async Task<IActionResult> CourseIndex()
        {
            var events = db.EventUsers.Where(x => x.ApplicationUserId == User.GetIdentityId()).Select(x=>x.EventId).ToList();
            var signups = db.CourseSignups.Where(x => x.ApplicationUserId == User.GetIdentityId()).Select(x => x.CourseId).ToList();

            Courses = await db.Courses
                .Include(x=>x.Participants)
                .Where(x => events.Contains(x.Id) || signups.Contains(x.Id))
                .Include(x => x.CourseSessions)
                    .ThenInclude(x=>x.CourseSessionTemplate)
                        .ThenInclude(x=>x.Address)
                .Include(x=>x.CourseSessions)
                    .ThenInclude(x=>x.Address)
                .Include(x => x.CourseSessions)
                    .ThenInclude(x => x.Divelocation)
                .OrderBy(x => x.StartDateAndTime)
                .ToListAsync();

            return View(Courses);
        }

        public async Task<IActionResult> GetRoute(int CourseSessionId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://dev.virtualearth.net");

            var endPoint = await db.CourseSessions
                .Include(x => x.CourseSessionTemplate).ThenInclude(x => x.Address)
                .Include(x=>x.Divelocation).ThenInclude(x=>x.MeetingLocation)
                .Include(x=>x.Address)
                .FirstOrDefaultAsync(x => x.Id == CourseSessionId);

            var endLocation = new Models.Structs.MapCoordinates();

            var endPointAddress = endPoint.Address;
            var endpointDiveLocationAdress = endPoint.Divelocation?.MeetingLocation;
            var endpointCourseSessionTemplateAddress = endPoint.CourseSessionTemplate?.Address;
            var endAddress = "";
            var endLocationName = "Mødested";

            //if (endPointAddress == null)
            //{
            if (endpointDiveLocationAdress == null)
            {
                if (endpointCourseSessionTemplateAddress == null)
                {
                    StatusMessage = "Ingen adresse på mødested/Dykkersted";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    endLocation.latitude = endpointCourseSessionTemplateAddress.Latitude;
                    endLocation.longitude = endpointCourseSessionTemplateAddress.Longitude;
                    endAddress = $"{endpointCourseSessionTemplateAddress.Streetname},{endpointCourseSessionTemplateAddress.City},{endpointCourseSessionTemplateAddress.Country}";
                    endLocationName = endpointCourseSessionTemplateAddress.Name;
                }
            }
            else
            {
                endLocation.latitude = endpointDiveLocationAdress.Latitude;
                endLocation.longitude = endpointDiveLocationAdress.Longitude;
                endAddress = $"{endpointDiveLocationAdress.Streetname},{endpointDiveLocationAdress.City},{endpointDiveLocationAdress.Country}";
                endLocationName = endpointDiveLocationAdress.Name;
            }

            //else
            //{
            //    endLocation.latitude = endPointAddress.Latitude;
            //    endLocation.longitude = endPointAddress.Longitude;
            //    endAddress = $"{endPointAddress.Streetname},{endPointAddress.City},{endPointAddress.Country}";
            //}


            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());
            if (string.IsNullOrEmpty(user.Streetaddress) && string.IsNullOrEmpty(user.City) && string.IsNullOrEmpty(user.Country))
            {
                StatusMessage = "Du manlger at angive din adresse, før vi kan lave en rutebeskrivlese";
                return LocalRedirect("/Identity/Account/Manage");
            }

            var startAddress = $"{user.Streetaddress},{user.City},{user.Country}";

            var model = new Models.Structs.RoutePoints()
            {
                End = endLocation,
                EndName = endLocationName,
                StartAddress = startAddress,
                EndAddress = endAddress
            };

            return View(model);


        }
    }
}