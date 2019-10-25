using System;
using System.Collections.Generic;
using System.Linq;
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

        [BindProperty]
        public IEnumerable<Event> Events { get; set; }

        public async Task<IActionResult> Index()
        {
            Events = await db.EventUsers.Include(x=>x.Event)
                .Where(x=> !x.Event.IsCancelled && x.ApplicationUserId == User.GetIdentityId())
                .Select(x=>x.Event).Include(x=>x.Participants)
                .OrderBy(x=>x.StartDateAndTime).ThenBy(x=>x.Id)
                .ToListAsync();

            return View(Events);
        }
    }
}