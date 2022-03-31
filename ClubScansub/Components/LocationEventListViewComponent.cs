using ClubScansub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Components
{
    public class LocationEventListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public LocationEventListViewComponent(ApplicationDbContext db)
        {
            this.db = db;

        }

        public async Task<IViewComponentResult> InvokeAsync(int locationId, bool isActive)
        {
            var data = await db.Divelocations
                .Include(x=>x.Events).ThenInclude(x=>x.Participants)
                .FirstOrDefaultAsync(x => x.Id == locationId)
                
                ;

            var evts = data.Events;
            if (isActive)
                evts = evts.Where(x => x.IsCancelled != isActive && x.StartDateAndTime > DateTime.Now).ToList();
            else
                evts = evts.Where(x => x.IsCancelled != isActive || x.EndDateAndTime < DateTime.Now).ToList();

            return View(evts.OrderBy(x=>x.StartDateAndTime));
        }
    }
}
