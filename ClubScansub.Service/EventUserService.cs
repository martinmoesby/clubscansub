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
    public class EventUserService :ServiceBase
    {
        private readonly ApplicationDbContext context;

        public EventUserService(IOptions<ServiceOptions> options, IEmailSender emailSender) 
            :base (options, emailSender) 
        {
                context = new ApplicationDbContext(dbContextOptions);
        }

        public async Task<IList<EventUser>> GetUsersByEvent(int Id)
        {
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
                    Description = $"Tilmelding til {currentEvent.Title} d. {currentEvent.StartDateAndTime.ToShortDateString()}",
                    Event = currentEvent
                });

            await context.EventUsers.AddAsync(new EventUser() { ApplicationUserId = selectedUser, EventId = EventId });
            await context.SaveChangesAsync();
            }
            return await GetUsersByEvent(EventId);
        }

        public async Task<IList<EventUser>> RemoveUserFromEvent(string userId, int courseId)
        {
            var eventUser = await context.EventUsers.FindAsync(userId, courseId);
            var currentEvent = await context.Events.FindAsync(courseId);
            var currentUser = await context.ApplicationUsers.FindAsync(userId); 

            if (eventUser != null && currentEvent != null && currentUser != null)
            {
                context.EventUsers.Remove(eventUser);
                //TODO: Refund any payments for this course made to users account...

                var accountEntry = await context.ApplicationUserAccountEntry.Where(x=>x.ApplicationUser == eventUser.ApplicationUser && x.Event.Id == courseId).FirstAsync();
                if (accountEntry != null)
                {
                    if (accountEntry.Amount != 0) {

                    var newAccountEntry = new ApplicationUserAccountEntry()
                    {
                        AccountType = Utility.AccountTypeEnum.EventAccountType,
                        Amount = -accountEntry.Amount,
                        ApplicationUser = currentUser,
                        PostingDate = DateTime.UtcNow,
                        Description = $"Refundering af {currentEvent.Title} d. {currentEvent.StartDateAndTime.ToShortDateString()}",
                        Event = eventUser.Event
                    };

                    await context.AddRangeAsync(newAccountEntry);
                    }
                }


                await context.SaveChangesAsync();
            }
            return await GetUsersByEvent(courseId);
        }

    }
}
