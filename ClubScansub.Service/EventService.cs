using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    public class EventService : ServiceBase, IEventService
    {
        private readonly ApplicationDbContext context;

        public EventService(IOptions<ServiceOptions> options, IEmailSender emailSender) : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
        }

        public Task<Event> AddAsync(Event Item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Event>> AddAsync(IList<Event> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Event>> AddAsync(params Event[] Items)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Event Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            var data = context.Events.Where(x=> x.StartDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public Task<Event> GetAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<Event> UpdateAsync(Event item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Event>> UpdateAsync(IList<Event> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Event>> UpdateAsync(params Event[] Items)
        {
            throw new NotImplementedException();
        }
    }
}
