using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

        public async Task<IList<Event>> GetActiveEventsByDivesiteId(int divesiteId)
        {
            var data = context.Events.Include(x=>x.Participants).Where(x => x.StartDateAndTime > DateTime.UtcNow && x.Divelocation.Id == divesiteId);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            var data = context.Events.Where(x=> x.EndDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllByDateAsync(DateTime startDate, DateTime endDate)
        {
            var data = context.Events.Include(x=>x.Divelocation).Where(x => x.StartDateAndTime >= startDate && x.EndDateAndTime <= endDate);
            return await data.ToListAsync();
        }

        public async Task<Event> GetAsync(string Id)
        {
            var id = int.Parse(Id);

            var data = await context.Events.Include(x => x.Divelocation).ThenInclude(x=>x.Image).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data == null)
                throw new Exception($"Event with Id '{Id}' could nor be retrieved.");

            return data;

        }

        public Task<Event> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Event>> GetCompletedEventsByDivesiteId(int divesiteId)
        {
            var data = context.Events.Include(x => x.Participants).Where(x => x.StartDateAndTime < DateTime.UtcNow && x.Divelocation.Id == divesiteId);
            return await data.ToListAsync();
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
