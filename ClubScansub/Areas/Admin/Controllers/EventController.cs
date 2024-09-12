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
            public IEnumerable<SelectListItem> MultiUsersList { get; set; }
            public IEnumerable<SelectListItem> LocationsList { get; set; }

        }

        public PageModel _PageModel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index(EventTypeEnum eventtype = EventTypeEnum.Bådtur)
        {
            var @events = await db.Events
                .Where(x => x.EventType == eventtype && x.StartDateAndTime > DateTime.Now)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.ExternalClubMembers).ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.Divelocation)
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
            ViewBag.EventType = eventtype;
            ViewBag.IsClosedEvents = false;
            ViewBag.StatusMessage = StatusMessage;
            return View(events);

        }

        public async Task<IActionResult> ClosedEvents(EventTypeEnum eventtype = EventTypeEnum.Bådtur)
        {
            var @events = await db.Events
                .Where(x => x.EventType == eventtype && x.StartDateAndTime <= DateTime.Now)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.ExternalClubMembers).ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.Divelocation)
                .ToListAsync();

            ViewBag.Pagetitle = "Afsluttede ";

            switch (eventtype)
            {
                case EventTypeEnum.Bådtur:
                    ViewBag.Pagetitle += "Bådture";
                    break;
                case EventTypeEnum.Stranddyk:
                    ViewBag.Pagetitle += "Stranddyk";
                    break;
                case EventTypeEnum.Rejse:
                    ViewBag.Pagetitle += "Dykkerture";
                    break;
                case EventTypeEnum.Liveaboard:
                    ViewBag.Pagetitle += "Liveaboard ferie";
                    break;
                case EventTypeEnum.Klubture:
                    ViewBag.Pagetitle += "Andre klubture";
                    break;
                case EventTypeEnum.Other:
                default:
                    ViewBag.Pagetitle += "Andre begivenheder";
                    break;
            }

            ViewBag.EventType = eventtype;

            ViewBag.IsClosedEvents = true;

            ViewBag.StatusMessage = StatusMessage;
            return View("Index", events);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await db.Events
                .Include(x => x.Participants)
                   .ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.ExternalClubMembers).ThenInclude(x=>x.ApplicationUser)
                   .Include(x => x.Divelocation)
                   .Include(x => x.SecondaryDivelocation)
                   .Include(x => x.RequiredCertificate)
                   .FirstOrDefaultAsync(x => x.Id == id);

            if (eventItem == null)
                return NotFound();

            _PageModel.Event = eventItem;
            if (eventItem.EventType == EventTypeEnum.Stranddyk || eventItem.EventType == EventTypeEnum.Klubture)
            {
                _PageModel.LocationsList = await db.Divelocations.Where(x => x.DefaultEventType == EventTypeEnum.Klubture || x.DefaultEventType == EventTypeEnum.Stranddyk).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
            }
            else
            {
                _PageModel.LocationsList = await db.Divelocations.Where(x => x.DefaultEventType == eventItem.EventType).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
            }

            //if (eventItem.RequiredCertificate != null)
            //    _PageModel.SelectedCertificate = eventItem.RequiredCertificate.Id;
            _PageModel.MultiUsersList = await db.ApplicationUsers.Where(x => x.IsMultiUser).OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).Select(x => new SelectListItem()
            {
                Text = $"{x.Name} ({x.AccountNumber})",
                Value = x.Id
            }).ToListAsync();

            _PageModel.UsersList = await db.ApplicationUsers.Where(x => !x.IsMultiUser).OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).Select(x => new SelectListItem()
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

            if (pageModel.Event.Divelocation != null)
                pageModel.Event.Divelocation = await db.Divelocations.FindAsync(pageModel.Event.Divelocation.Id);

            if (pageModel.Event.SecondaryDivelocation != null)
                pageModel.Event.SecondaryDivelocation = await db.Divelocations.FindAsync(pageModel.Event.SecondaryDivelocation.Id);



            if (ModelState.IsValid)
            {
                var existingEvent = await db.Events
                    .Include(x => x.Divelocation)
                    .Include(x => x.SecondaryDivelocation)
                    .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == pageModel.Event.Id);

                var newLocation = await db.Divelocations.FindAsync(pageModel.Event.Divelocation.Id);
                var newLoc2 = await db.Divelocations.FindAsync(pageModel.Event.SecondaryDivelocation?.Id);
                // Notify uisers of new divelocation

                if (existingEvent.Divelocation.Id != pageModel.Event.Divelocation.Id || existingEvent.SecondaryDivelocation?.Id != pageModel.Event.SecondaryDivelocation?.Id)
                {
                    // Rename Titel and Details of event
                    pageModel.Event.Title = $"{pageModel.Event.EventType} til {newLocation.Name}";

                    if (newLoc2 != null)
                    {
                        pageModel.Event.Title += $" og {newLoc2.Name}";
                    }


                    pageModel.Event.Details = $"{pageModel.Event.Title}. Turen kræver mindst {pageModel.Event.MinParticipants} deltagere og der er plads til maksimalt {pageModel.Event.MaxParticipants}";

                    // Notify signed up users using smsSender and emailSender
                    var participants = await db.EventUsers.Where(x => x.EventId == pageModel.Event.Id).Select(x => x.ApplicationUser).ToListAsync();

                    foreach (var participant in participants)
                    {
                        var smsText = $"Hej {participant.Firstname}, \n\n" +
                            $"Turen til '{existingEvent.Title}' den {pageModel.Event.StartDateAndTime} er blevet ændret til '{newLocation.Name}' \n" +
                            $"\n" +
                            $"Hvis du ikke har mulighed eller lyst til at deltage, så kontakt venligst klubben for at få dig afmeldt (ub) \n" +
                            $"\n" +
                            $"Med venlig hilsen\n" +
                            $"Klub Scansub";
                        await smsSender.SendSmsAsync(participant.PhoneNumber, smsText);

                    }

                }

                // Notify users of new event date
                if (existingEvent.StartDateAndTime != pageModel.Event.StartDateAndTime)
                {
                    // Notify signed up users using smsSender and emailSender
                    var participants = await db.EventUsers.Where(x => x.EventId == pageModel.Event.Id).Select(x => x.ApplicationUser).ToListAsync();

                    foreach (var participant in participants)
                    {
                        var smsText = $"Hej {participant.Firstname}, \n\n" +
                            $"Turen til '{existingEvent.Title}' den {existingEvent.StartDateAndTime.ToShortDateString()} er blevet ændret til '{pageModel.Event.StartDateAndTime.ToShortDateString()}' \n" +
                            $"\n" +
                            $"Hvis du ikke har mulighed for at deltage på turen denne dag, så kontakt venligst klubben for at blive afmeldt turen. \n" +
                            $"\n" +
                            $"Med venlig hilsen\n" +
                            $"Klub Scansub";
                        await smsSender.SendSmsAsync(participant.PhoneNumber, smsText);

                    }

                }


                //if (pageModel.SelectedCertificate != null)
                //    pageModel.Event.RequiredCertificate = await db.Certificates.FindAsync(pageModel.SelectedCertificate);

                db.Attach(pageModel.Event).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                if (existingEvent.SecondaryDivelocation != null && pageModel.Event.SecondaryDivelocation == null)
                {
                    var ev = await db.Events.Include(x => x.SecondaryDivelocation).SingleAsync(x => x.Id == pageModel.Event.Id);

                    db.Entry(ev).Property("SecondaryDivelocationId").CurrentValue = null;
                    db.Entry(ev).Property("SecondaryDivelocationId").IsModified = true;

                    //db.Entry(blog).Property("PostId").CurrentValue = null;
                    //This tells EF that you know the value of PostId and that it is null--previously even though the value was null EF had a flag set indicating that the value was unknown.
                    //I think you should also be able to do this:
                    //db.Entry(blog).Property("PostId").IsModified = true;
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index", new { eventtype = pageModel.Event.EventType });
            }

            //_PageModel.Event = pageModel.Event;
            pageModel.Event.Participants = await db.EventUsers.Where(x => x.EventId == pageModel.Event.Id).Include(x => x.ApplicationUser).ToListAsync();
            pageModel.Event.Divelocation = await db.Divelocations.FindAsync(pageModel.Event.Divelocation.Id);
            pageModel.Event.SecondaryDivelocation = await db.Divelocations.FindAsync(pageModel.Event.SecondaryDivelocation?.Id);
            pageModel.LocationsList = await db.Divelocations.Where(x => x.DefaultEventType == pageModel.Event.EventType).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();
            pageModel.UsersList = await db.ApplicationUsers.OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id
            }).ToListAsync();

            _PageModel.CertificatesList = await db.Certificates.Select(x => new SelectListItem()
            {
                Text = $"{x.ShortName} ({x.DepthLimit} m.)",
                Value = x.Id.ToString()
            }).ToListAsync();

            ViewBag.StatusMessage = StatusMessage;

            return View(pageModel);

        }

        [HttpPost, ActionName("CreateEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel newevent)
        {

            var location = await db.Divelocations.FindAsync(newevent.DivelocationId);
            var certificate = await db.Certificates.FindAsync(newevent.CertificateId);

            var item = newevent.Event;

            if (item.Price == 0)
                item.Price = location.Price;

            item.Divelocation = location;
            item.RequiredCertificate = certificate;
            item.DeeplinkId = Guid.NewGuid();

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

        [HttpGet] 
        public async Task<ActionResult> Create(EventTypeEnum eventType)
        {
            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysInFuture = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;

            DateTime eventDate = today.AddDays(daysInFuture).AddHours(8).AddMinutes(30);

            _PageModel.Event = new Event()
            {
                StartDateAndTime = eventDate,
                EndDateAndTime = eventDate.AddHours(4),
            };
            
            _PageModel.CertificatesList = await db.Certificates.Select(x => new SelectListItem()
            {
                Text = $"{x.ShortName} ({x.DepthLimit} m.)",
                Value = x.Id.ToString()
            }).ToListAsync();
            _PageModel.LocationsList = await db.Divelocations.Where(x => x.DefaultEventType == eventType).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync();

            return View(_PageModel);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PageModel pagemodel)
        {
            //if (!ModelState.IsValid) 
            //    return View(pagemodel);

            db.Events.Add(pagemodel.Event);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index),pagemodel.Event.EventType);
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


        public async Task<IActionResult> ConfirmRemoveUserFromEvent(int eventid, string userid)
        {
            var ev_user = await db.EventUsers.Include(x=>x.Event).Include(x=>x.ApplicationUser).FirstOrDefaultAsync(x=>x.ApplicationUserId == userid && x.EventId == eventid);
            if(ev_user != null)
            {
                return PartialView("_ConfirmRemoveUserPartial",ev_user);
            }
            return RedirectToAction("Edit", new { id = eventid });
        }

        public async Task<IActionResult> RemoveExternalUserFromEvent(int eventid, string userid)
        {
            var ev_user = await db.EventExternalUsers.Where(x => x.EventId == eventid && x.ApplicationUserId == userid).FirstOrDefaultAsync();

            if (ev_user != null)
            {
                db.EventExternalUsers.Remove(ev_user);
                var user = await db.ApplicationUsers
                    .Include(x => x.AccountTransactions)
                        .ThenInclude(x => x.Event)
                    .FirstOrDefaultAsync(x => x.Id == userid);

                var ev = await db.Events.FindAsync(eventid);
                decimal refundFactor = 1m;

                if ((ev.StartDateAndTime - DateTime.Now).Days > 0 && (ev.StartDateAndTime - DateTime.Now).Days < 8)
                    refundFactor = 0.5m;

                if ((ev.StartDateAndTime - DateTime.Now).Days < 1)
                    refundFactor = 0;

                if (user.PhoneNumberConfirmed)
                {
                    var smsSendResult = await smsSender.SendSmsAsync(user.PhoneNumber, $"Du har afmeldt en dykker til '{ev.Title}' d. {ev.StartDateAndTime.ToShortDateString()}. Du er blevet refunderet {refundFactor * 100} % af betalingen for turen");
                }

                var newEntry = new ApplicationUserAccountEntry
                {
                    Amount = ev.Price ?? 0 * refundFactor,
                    ApplicationUser = user,
                    AccountType = AccountTypeEnum.EventAccountType,
                    Event = ev,
                    Description = $"Tilbageførsel for en dykker '{ev.Title}' ({refundFactor * 100} %)",
                    PostingDate = DateTime.Now
                };
                db.ApplicationUserAccountEntry.Add(newEntry);

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
            var userprice = await um.IsInRoleAsync(applicationUser, Userroles.Member) ? @event.PremiumPrice : @event.Price;

            @event.Participants.Add(new EventUser() { ApplicationUserId = userId });

            if (doCreateAccountTransaction)
            {
                db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                {
                    AccountType = AccountTypeEnum.EventAccountType,
                    Amount = -userprice ?? 0,
                    Description = $"Betaling for {@event.Title}",
                    PostingDate = DateTime.Now,
                    Event = @event,
                    ApplicationUser = applicationUser
                });
            }

            if (applicationUser.PhoneNumberConfirmed)
            {
                var smsMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du er nu blevet tilmeldt turen {@event.Title} d. {@event.StartDateAndTime}.\n" +
                    $"\n" +
                    $"Vi glæder os rgtig meget til at se dig.\n" +
                    $"\n" +
                    $"Med venlig hilsen \n" +
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


        [HttpPost, ActionName("AddExternalUserToEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExternalUsersToEvent(int eventId, string userId, int numberofseats)
        {

            var @event = await db.Events
                .Include(x => x.ExternalClubMembers)
                .FirstOrDefaultAsync(x => x.Id == eventId);

            var applicationUser = await db.ApplicationUsers.FindAsync(userId);
            var userprice = @event.Price;

            for (int i = 0; i < numberofseats; i++)
            {
                @event.ExternalClubMembers.Add(new EventMultiApplicationUser() { ApplicationUser = applicationUser });
            }

            db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
            {
                AccountType = AccountTypeEnum.EventAccountType,
                Amount = -userprice ?? 0 * numberofseats,
                Description = $"Betaling for {numberofseats} dykkere til {@event.Title}",
                PostingDate = DateTime.Now,
                Event = @event,
                ApplicationUser = applicationUser
            });

            if (applicationUser.PhoneNumberConfirmed)
            {
                var smsMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du har fået tilmeldt {numberofseats} dykkere til turen {@event.Title} d. {@event.StartDateAndTime}.\n" +
                    $"\n" +
                    $"Vi glæder os rigtig meget til at se jer.\n" +
                    $"\n" +
                    $"Med venlig hilsen \n" +
                    $"Scansub DK Diver";

                await smsSender.SendSmsAsync(applicationUser.PhoneNumber, smsMessage);
                StatusMessage = $"{applicationUser.Firstname} er blevet tilmeldt og har fået beksed via SMS";
            }

            if (applicationUser.EmailConfirmed)
            {
                var mailMessage = $"Hej {applicationUser.Firstname}," +
                    $"Du har fået tilmeldt {numberofseats} dykkere til turen {@event.Title} med start d. {@event.StartDateAndTime}.<br />" +
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
                .Include(x=>x.ExternalClubMembers).ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.Divelocation)
                .FirstOrDefaultAsync(x => x.Id == id);

            return View(events);
        }

        public async Task<IActionResult> FixedUser(int eventid, bool isadding, int numberOfSeats = 1)
        {
            var e = await db.Events.FindAsync(eventid);

            e.FixedParticipants = isadding ? e.FixedParticipants+=numberOfSeats: e.FixedParticipants-=numberOfSeats;

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = e.Id });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var e = await db.Events
                    .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                    .Include(x=>x.ExternalClubMembers).ThenInclude(x => x.ApplicationUser)
                    .FirstOrDefaultAsync(x => x.Id == id);

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


            var externalClubParticipants = e.ExternalClubMembers.GroupBy(x => x.ApplicationUser).Select(group => new { User = group.Key, Count = group.Count() }).OrderBy(x => x.User);
            foreach (var item in externalClubParticipants)
            {
                var accountTransaction = item.Count * e.Price;
                //var accountTransaction = await db.ApplicationUserAccountEntry
                //    .Include(x => x.ApplicationUser)
                //    .Include(x => x.Event)
                //    .OrderByDescending(x => x.Id)
                //    .Where(x => x.Event.Id == id && x.ApplicationUser.Id == item.User.Id).SumAsync(x=>x.Amount);

                if (accountTransaction != 0)
                {
                    var accounttrans = new ApplicationUserAccountEntry
                    {
                        Amount = accountTransaction ?? 0,
                        AccountType = AccountTypeEnum.EventAccountType,
                        PostingDate = DateTime.Now,
                        Description = $"Tilbageførsel pga. annulering af '{e.Title}' for {item.Count} dykkere",
                        ApplicationUser = item.User,
                        Event = e
                    };
                    db.ApplicationUserAccountEntry.Add(accounttrans);
                }
            }


            var smsSendResult = await smsSender.SendMultipleSmsAsync(e.Participants.Select(x=>x.ApplicationUser), $"Begivenheden '{e.Title}' d. {e.StartDateAndTime.ToShortDateString()} er desværre blevet aflyst. \n\nEvt. pris er blevet indstat på din turkonto\n\nMed venlig hilsen\nKlub Scansub");
            var smsSendResult2 = await smsSender.SendMultipleSmsAsync(externalClubParticipants.Select(x => x.User), $"Begivenheden '{e.Title}' d. {e.StartDateAndTime.ToShortDateString()} er desværre blevet aflyst. \n\nVi har tilbageført prisen for de tilmeldte dykkere.\n\nMed venlig hilsen\nKlub Scansub");

            if (smsSendResult.Any(x => !x.IsSuccessStatusCode) || smsSendResult2.Any(x => !x.IsSuccessStatusCode))
            {
                StatusMessage = "Der opstod fejl i forbindelse med afsendelsen af SMS: \n";
                          
                foreach (var item in smsSendResult.Where(x => !x.IsSuccessStatusCode))
                {
                    StatusMessage += $"{item.GetErrorMessage()}\n";
                }
                foreach (var item in smsSendResult2.Where(x => !x.IsSuccessStatusCode))
                {
                    StatusMessage += $"{item.GetErrorMessage()}\n";
                }
            }

            e.IsCancelled = true;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventtype });

        }

        public async Task<IActionResult> CompleteDelete(int id)
        {

            if (!User.IsInRole("Administrator"))
                return new StatusCodeResult(403);
                
            var e = await db.Events
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.ExternalClubMembers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var eventtype = e.EventType;
            db.Remove(e);

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventtype });

        }
    }
}