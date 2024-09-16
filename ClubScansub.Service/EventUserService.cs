using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
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
            await context.EventUsers.AddAsync(new EventUser() { ApplicationUserId = selectedUser, EventId = EventId });
            await context.SaveChangesAsync();
            return await GetUsersByEvent(EventId);
        }
        public async Task<IList<EventUser>> RemoveUserFromEvent(string userId, int courseId)
        {
            var eventUser = await context.EventUsers.FindAsync(userId, courseId);
            if (eventUser != null)
            {
                context.EventUsers.Remove(eventUser);
                //TODO: Refund any payments for this course made to users account...
                await context.SaveChangesAsync();
            }
            return await GetUsersByEvent(courseId);
        }

    }
}
