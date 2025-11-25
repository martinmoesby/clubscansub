using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using Mailjet.Client.Resources.SMS;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    public class EventUserService : ServiceBase
    {
        private readonly ApplicationDbContext context;

        public EventUserService(IOptions<ServiceOptions> options, IEmailSender emailSender) 
            :base (options, emailSender) 
        {
                context = new ApplicationDbContext(dbContextOptions);
        }

        public async Task<IList<EventUser>> GetUsersByEvent(int Id)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var users = await context.EventUsers.Include(x => x.ApplicationUser)
                .Where(x => x.EventId == Id).ToListAsync();
            return users;
        }
        public async Task<IList<EventUser>> AddUserToEvent(string selectedUser, int EventId)
        {
            return await AddUserToEvent(selectedUser, EventId, true);
            //await context.EventUsers.AddAsync(new EventUser() { ApplicationUserId = selectedUser, EventId = EventId });
            //await context.SaveChangesAsync();
            //return await GetUsersByEvent(EventId);
        }


        public async Task<IList<EventUser>> AddUserToEvent(string selectedUser, int EventId, bool subtractEventPriceFromBalance)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            try
            {
                var currentEvent = await context.Events.FindAsync(EventId);
                var currentMember = await context.ApplicationUsers.FindAsync(selectedUser);
                if (currentEvent != null && currentMember != null)
                {
                    var amount = currentEvent.PremiumPrice ?? currentEvent.Price ?? 0;

                    await context.ApplicationUserAccountEntry.AddAsync(new ApplicationUserAccountEntry()
                    {
                        AccountType = Utility.AccountTypeEnum.EventAccountType,
                        Amount = subtractEventPriceFromBalance ? -amount : 0,
                        ApplicationUser = currentMember,
                        PostingDate = DateTime.UtcNow,
                        Description = $"Tilmelding til {currentEvent.Title} d. {currentEvent.StartDateAndTime:d}",
                        Event = currentEvent
                    });

                    await context.EventUsers.AddAsync(new EventUser() { ApplicationUserId = selectedUser, EventId = EventId });
                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add user to event", ex);
            }            
            return await GetUsersByEvent(EventId);
        }

        public async Task<IList<EventUser>> RemoveUserFromEvent(string userId, int eventId)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var eventUser = await context.EventUsers.FindAsync(userId, eventId);
            if (eventUser != null  ) //&& currentEvent != null && currentUser != null)
            {
                context.EventUsers.Remove(eventUser);
                await context.SaveChangesAsync();
                await RefundForEvent(eventUser);
            }
            return await GetUsersByEvent(eventId);
        }

        public async Task RefundForEvent(EventUser eventUser, bool refundFullAmount = false)
        {
            var currentEvent = await context.Events.FindAsync(eventUser.EventId);

            var accountEntry = await context.ApplicationUserAccountEntry.Where(x => x.ApplicationUser == eventUser.ApplicationUser && x.Event.Id == eventUser.EventId).FirstOrDefaultAsync();

            if (accountEntry != null && currentEvent != null)
            {
                if (accountEntry.Amount != 0)
                {
                    decimal refundFactor = 1m;
                    if (!refundFullAmount)
                    {

                        if ((currentEvent.StartDateAndTime - DateTime.Now).Days > 0 && (currentEvent.StartDateAndTime - DateTime.Now).Days < 8)
                            refundFactor = 0.5m;

                        if ((currentEvent.StartDateAndTime - DateTime.Now).Days < 1)
                            refundFactor = 0;
                    }

                    var newAccountEntry = new ApplicationUserAccountEntry()
                    {
                        AccountType = Utility.AccountTypeEnum.EventAccountType,
                        Amount = -accountEntry.Amount*refundFactor,
                        ApplicationUser = eventUser.ApplicationUser,
                        PostingDate = DateTime.UtcNow,
                        Description = $"Refundering af {currentEvent.Title} d. {currentEvent.StartDateAndTime:d}",
                        Event = eventUser.Event
                    };

                    await context.AddRangeAsync(newAccountEntry);
                }
            }
        }

        public async Task RefundForCancelledEvent(Event @event)
        {
            var eventUsers = await context.EventUsers.Where(x => x.EventId == @event.Id).ToListAsync();
            if (eventUsers != null)
            {
                foreach (var item in eventUsers)
                {
                    await RefundForEvent(item, true);
                }
            }
        }
    }
}
