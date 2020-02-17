using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> um;
        private readonly ISmsSender smsSender;
        private readonly IEmailSender emailSender;

        public EventController(ApplicationDbContext db, UserManager<IdentityUser> um, ISmsSender smsSender, IEmailSender emailSender)
            : base(db)
        {
            this.um = um;
            this.smsSender = smsSender;
            this.emailSender = emailSender;

            _PageModel = new PageModel();
        }

        public class PageModel
        {
            public Event Event { get; set; }
            public IEnumerable<SelectListItem> UsersList { get; set; }
            public IEnumerable<SelectListItem> CertificatesList { get; set; }

        }

        public PageModel _PageModel { get; set; }

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
            var eventItem = await db.Events
                .Include(x => x.Participants)
                    .ThenInclude(x => x.ApplicationUser)
                   .Include(x => x.Divelocation)
                   .Include(x=>x.RequiredCertificate)
                   .FirstOrDefaultAsync(x => x.Id == id);

            if (eventItem == null)
                return NotFound();

            _PageModel.Event = eventItem;

            //if (eventItem.RequiredCertificate != null)
            //    _PageModel.SelectedCertificate = eventItem.RequiredCertificate.Id;

            _PageModel.UsersList = await db.ApplicationUsers.OrderBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = $"{x.Name} ({x.AccountNumber})",
                Value = x.Id
            }).ToListAsync();

            _PageModel.CertificatesList = await db.Certificates.Select(x => new SelectListItem() 
            {
                Text = $"{x.ShortName} ({x.DepthLimit} m.)",
                Value = x.Id.ToString()
            }).ToListAsync();

            return View(_PageModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PageModel pageModel)
        {
            if (pageModel.Event.RequiredCertificate != null)
                pageModel.Event.RequiredCertificate = await db.Certificates.FindAsync(pageModel.Event.RequiredCertificate.Id);

            if (ModelState.IsValid)
            {
                //if (pageModel.SelectedCertificate != null)
                //    pageModel.Event.RequiredCertificate = await db.Certificates.FindAsync(pageModel.SelectedCertificate);
                
                db.Attach(pageModel.Event).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { eventtype = pageModel.Event.EventType });
            }

            //_PageModel.Event = pageModel.Event;
            pageModel.Event.Participants = await db.EventUsers.Where(x => x.EventId == pageModel.Event.Id).Include(x => x.ApplicationUser).ToListAsync();
            pageModel.Event.Divelocation = await db.Divelocations.FindAsync(pageModel.Event.Divelocation.Id);
            pageModel.UsersList = await db.ApplicationUsers.OrderBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id
            }).ToListAsync();

            ViewBag.StatusMessage = StatusMessage;

            return View(pageModel);

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
                var user = await db.ApplicationUsers
                    .Include(x=>x.AccountTransactions)
                        .ThenInclude(x=>x.Event)
                    .FirstOrDefaultAsync(x=>x.Id == userid);

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

                var accountEntry = user.AccountTransactions
                    .Where(x=>x.Event != null && x.AccountType == AccountTypeEnum.EventAccountType)
                    .OrderBy(x=>x.PostingDate)
                    .FirstOrDefault(x => x.Event.Id == eventid);

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

        [HttpPost, ActionName("AddUserToEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToEvent(int eventId, string userId, bool doCreateAccountTransaction = false)
        {

            var @event = await db.Events
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == eventId);

            var applicationUser = await db.ApplicationUsers.FindAsync(userId);

            @event.Participants.Add(new EventUser() { ApplicationUserId = userId });

            if (doCreateAccountTransaction)
            {
                db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                {
                    AccountType = AccountTypeEnum.EventAccountType,
                    Amount = -@event.Price,
                    Description = $"Betaling for {@event.Title}",
                    PostingDate = DateTime.Now,
                    Event = @event,
                    ApplicationUser = applicationUser
                });
            }

            if (applicationUser.PhoneNumberConfirmed)
            {
                var smsMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du er nu blevet tilmeldt turen {@event.Title} d. {@event.StartDateAndTime}." +
                    $"" +
                    $"" +
                    $"" +
                    $"Vi glæder os rgtig meget til at se dig." +
                    $"" +
                    $"Med venlig hilsen " +
                    $"Scansub DK Diver";

                await smsSender.SendSmsAsync(applicationUser.PhoneNumber, smsMessage);
                StatusMessage = $"{applicationUser.Firstname} er blevet tilmeldt og har fået beksed via SMS";
            }

            if (applicationUser.EmailConfirmed)
            {
                var mailMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du er nu blevet tilmeldt turen {@event.Title} med start d. {@event.StartDateAndTime}.<br />" +
                    $"" +
                    $"" +
                    $"<br />" +
                    $"Vi glæder os rgtig meget til at se dig.<br/>" +
                    $"<br />" +
                    $"<br/><br/>Med venlig hilsen <br/><br/> Scansub DK Diver";

                await emailSender.SendEmailAsync(applicationUser.Email, $"Tilmelding til {@event.Title}", mailMessage);
                StatusMessage = $"{applicationUser.Firstname} er blevet tilmeldt og har fået besked vial mail";
            }

            if (!applicationUser.EmailConfirmed && !applicationUser.PhoneNumberConfirmed)
            {
                StatusMessage = $"Fejl: {applicationUser.Firstname} har hverken valideret sin email eller sit telefonr. så det har ikke været muligt at give elektronisk besked.";
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = eventId });
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
            var e = await db.Events.Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser).FirstOrDefaultAsync(x=>x.Id == id);
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

            var smsSendResult = await smsSender.SendMultipleSmsAsync(e.Participants.Select(x=>x.ApplicationUser), $"Begivenheden '{e.Title}' d. {e.StartDateAndTime.ToShortDateString()} er desværre blevet aflyst. \n\nEvt. pris er blevet indstat på din turkonto\n\nMed venlig hilsen\nKlub Scansub");

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