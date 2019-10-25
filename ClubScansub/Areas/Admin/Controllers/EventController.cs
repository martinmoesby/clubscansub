using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> um;
        private readonly ISmsSender smsSender;
        public EventController(ApplicationDbContext db, UserManager<IdentityUser> um, ISmsSender smsSender)
            : base(db)
        {
            this.um = um;
            this.smsSender = smsSender;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index(EventTypeEnum eventtype = EventTypeEnum.Bådtur)
        {
            var @events = await db.Events
                .Where(x=>x.EventType == eventtype && x.IsCancelled == false)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.Divelocation)
                .ToListAsync();


            switch (eventtype)
            {
                case EventTypeEnum.Bådtur:
                    ViewBag.Pagetitle = "Bådture";
                    break;
                case EventTypeEnum.Stranddyk:
                    ViewBag.Pagetitle = "Stranddyk";
                    break;
                case EventTypeEnum.Rejse:
                    ViewBag.Pagetitle = "Dykkerture";
                    break;
                case EventTypeEnum.Liveaboard:
                    ViewBag.Pagetitle = "Liveaboard ferie";
                    break;
                case EventTypeEnum.Klubture:
                    ViewBag.Pagetitle = "Andre klubture";
                    break;
                case EventTypeEnum.Other:
                default:
                    ViewBag.Pagetitle = "Andre begivenheder";
                    break;
            }

            ViewBag.StatusMessage = StatusMessage;
            return View(events);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await db.Events.Include(x => x.Participants).ThenInclude(x => x.ApplicationUser).Include(x => x.Divelocation).FirstOrDefaultAsync(x => x.Id == id);

            if (eventItem == null)
                return NotFound();

            return View(eventItem);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event item)
        {

            if (ModelState.IsValid)
            {
                
                db.Attach(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { eventtype = item.EventType });
            }

            item.Participants = await db.EventUsers.Where(x => x.EventId == item.Id).Include(x => x.ApplicationUser).ToListAsync();
            item.Divelocation = await db.Divelocations.FindAsync(item.Divelocation.Id);
            return View(item);

        }

        [HttpPost, ActionName("CreateEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel newevent)
        {

            var location  = await db.Divelocations.FindAsync(newevent.DivelocationId);
            var certificate = await db.Certificates.FindAsync(newevent.CertificateId);

            var item = newevent.Event;

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
                        break;
                    case EventTypeEnum.Stranddyk:
                        item.Title = $"Strandtur til {location.Name}.";
                        break;
                    case EventTypeEnum.Rejse:
                        item.Title = $"Rejse til {location.Name}.";
                        break;
                    case EventTypeEnum.Liveaboard:
                        item.Title = $"Liveaboard tur til {location.Name}";
                        break;
                    default:
                        item.Title = "Ny tur";
                        break;
                }
            }

            item.Details += $"{item.Title.TrimEnd('.')} - kræver mindst { item.MinParticipants} deltagere og der er plads til maksimalt { item.MaxParticipants}";

            db.Events.Add(item);

            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RemoveUserFromEvent(int eventid, string userid)
        {
            var ev_user = await db.EventUsers.FindAsync(userid, eventid);
            if (ev_user != null)
            {
                db.EventUsers.Remove(ev_user);
                var user = await db.ApplicationUsers.FirstOrDefaultAsync(x=>x.Id == userid);
                var ev = await db.Events.FindAsync(eventid);
                decimal refundFactor = 1m;

                if ((ev.StartDateAndTime - DateTime.Now).Days > 0 && (ev.StartDateAndTime-DateTime.Now).Days < 8)
                    refundFactor = 0.5m;

                if ((ev.StartDateAndTime - DateTime.Now).Days < 1)
                    refundFactor = 0;

                if (user.PhoneNumberConfirmed)
                {
                    var smsSendResult = await smsSender.SendSmsAsync(user.PhoneNumber, $"Du er blevet afmeldt turen '{ev.Title}' d. {ev.StartDateAndTime.ToShortDateString()}. Du er blevet refunderet {refundFactor *100} % af din betaling for turen");
                }

                var accountEntry = await db.ApplicationUserAccountEntry.Include(x=>x.Event).FirstOrDefaultAsync(x => x.Event.Id == eventid);
                if (accountEntry != null)
                {
                    var newEntry = new ApplicationUserAccountEntry
                    {
                        Amount = -accountEntry.Amount * refundFactor,
                        ApplicationUser = user,
                        AccountType = AccountTypeEnum.EventAccountType,
                        Event = ev,
                        Description = $"Tilbageførsel for '{ev.Title}' ({refundFactor * 100} %)",
                        PostingDate = DateTime.Now
                    };
                    db.ApplicationUserAccountEntry.Add(newEntry);
                }
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = eventid });

        }

        [HttpGet]
        public async Task<JsonResult> GetLocationPrice(int locationid)
        {
            var location = await db.Divelocations.FindAsync(locationid);
            return new JsonResult(location.Price);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var @events = await db.Events
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Divelocation)
                .FirstOrDefaultAsync(x => x.Id == id);

            return View(events);
        }

        public async Task<IActionResult> FixedUser(int eventid, bool isadding)
        {
            var e = await db.Events.FindAsync(eventid);

            e.FixedParticipants = isadding ? ++e.FixedParticipants: --e.FixedParticipants;

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = e.Id });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var e = await db.Events.Include(x=>x.Participants).FirstOrDefaultAsync(x=>x.Id == id);
            var eventtype = e.EventType;

            foreach (var item in e.Participants)
            {
                var accountTransaction = await db.ApplicationUserAccountEntry
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Event)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.Event.Id == id && x.ApplicationUser.Id == item.ApplicationUserId);

                if (accountTransaction != null)
                {
                    var accounttrans = new ApplicationUserAccountEntry
                    {
                        Amount = -accountTransaction.Amount,
                        AccountType = AccountTypeEnum.EventAccountType,
                        PostingDate = DateTime.Now,
                        Description = $"Tilbageførsel pga. annulering af '{e.Title}'",
                        ApplicationUser = item.ApplicationUser,
                        Event = e
                    };
                    db.ApplicationUserAccountEntry.Add(accounttrans);
                }
            }

            var smsSendResult = await smsSender.SendMultipleSmsAsync(e.Participants.Select(x=>x.ApplicationUser), $"Turen '{e.Title}' d. {e.StartDateAndTime.ToShortDateString()} er desværre blevet aflyst. Du er blevet refunderet 100% af din betaling for turen");

            if (smsSendResult.Any(x=>!x.IsSuccessStatusCode))
            {
                StatusMessage = "Der opstod fejl i forbindelse med afsendelsen af SMS: ";
                foreach (var item in smsSendResult.Where(x => !x.IsSuccessStatusCode))
                {
                    StatusMessage += item.GetErrorMessage();
                }
            }
            e.IsCancelled = true;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventtype });

        }
    }
}