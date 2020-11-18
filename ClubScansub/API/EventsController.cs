using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : BaseController
    {
        public EventsController(ApplicationDbContext db) : base(db)
        {

        }

        public async Task<ActionResult> Get()
        {
            try
            {
                var data = await db.Events
                .Include(x => x.Divelocation).ThenInclude(x => x.MeetingLocation)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.IsCancelled == false && x.FreeSpots > 0 && x.StartDateAndTime > DateTime.Now && x.EventType!= Utility.EventTypeEnum.NotAnEvent)
                .Select(x => new
                {
                    Name = x.Title,
                    Desription = x.Details,
                    Date = x.StartDateAndTime.ToShortDateString(),
                    Time = x.StartDateAndTime.ToShortTimeString(),
                    Participants = x.Participants.Count,
                    AvailableSeats = x.FreeSpots,
                    x.Divelocation.Image,
                    x.Divelocation.MeetingLocation,
                    x.Price,
                    Id = x.Id.ToString(),
                    MinimumSeatRequirement = x.RequiredSpots
                }).ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
