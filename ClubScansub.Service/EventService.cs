using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using ClubScansub.Utility;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyModel;
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

        public async Task<IList<Event>> AddAsync(IList<Event> Items)
        {
            await context.AddRangeAsync(Items);
            await context.SaveChangesAsync();
            return Items;
        }

        public async Task<IList<Event>> AddAsync(params Event[] Items)
        {
            return await AddAsync(Items.ToList());
        }

        public async Task DeleteAsync(Event Item)
        {
            context.Events.Remove(Item);
            await context.SaveChangesAsync();
        }

        public async Task<IList<Event>> GetActiveEventsByDivesiteId(int divesiteId)
        {
            var data = context.Events.Include(x=>x.Participants).Where(x => x.StartDateAndTime > DateTime.UtcNow && x.Divelocation.Id == divesiteId);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            var data = context.Events.Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser).Where(x=>x.EventType != Utility.EventTypeEnum.NotAnEvent);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllByDateAsync(DateTime startDate, DateTime endDate)
        {
            var data = await context.Events.Include(x=>x.Participants).Include(x=>x.ExternalClubMembers).Include(x=>x.Divelocation).Where(x => x.StartDateAndTime >= startDate && x.EndDateAndTime <= endDate && x.EventType != Utility.EventTypeEnum.NotAnEvent).ToListAsync<Event>();
            return data;
        }

        public async Task<IList<CourseSession>> GetAllCourseSessionsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var data = await context.CourseSessions.Include(x=>x.Course).ThenInclude(x=>x.Participants).Where(x=>x.DateTime >= startDate && x.DateTime <= endDate).ToListAsync();
            return data;
        }

        public async Task<Event> GetAsync(string Id)
        {
            var id = int.Parse(Id);

            var data = await context.Events.Include(x=>x.Participants).Include(x => x.Divelocation).ThenInclude(x=>x.Image).Where(x => x.Id == id).FirstOrDefaultAsync();
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

        public async Task<Event> UpdateAsync(Event item)
        {
            context.Update(item);
            await context.SaveChangesAsync();
            return item;

        }

        public Task<IList<Event>> UpdateAsync(IList<Event> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Event>> UpdateAsync(params Event[] Items)
        {
            throw new NotImplementedException();
        }

        public async Task CreateEventsAsync(IList<Event> events)
        {
            foreach (var @event in events)
            {

                var location = context.Divelocations.Include(x => x.Certificate).Include(x => x.MeetingLocation).FirstOrDefault(x => x.Id ==@event.Divelocation.Id);
                if (location != null)
                {
                    var certificate = location.Certificate;

                    var item = new Event();
                    item.EventType = location.DefaultEventType;
                    item.StartDateAndTime = @event.StartDateAndTime;
                    item.MinParticipants = location.MinParticipants;
                    item.MaxParticipants = location.MaxParticipants;
                    item.FixedParticipants = 0;
                    item.EndDateAndTime = @event.EndDateAndTime;
                    item.Address = location.MeetingLocation;
                    item.DeeplinkId = Guid.NewGuid();
                    item.Details += $"{@event.Title?.TrimEnd('.')} - kræver mindst {@event.MinParticipants} deltagere og der er plads til maksimalt {@event.MaxParticipants}";

                    if (@event.Price == 0)
                    {
                        item.Price = location.Price;
                        item.PremiumPrice = item.EventType != EventTypeEnum.Klubture ? location.Price : 0;
                    } else
                    {
                        item.PremiumPrice = @event.PremiumPrice;
                        item.Price = @event.Price;
                    }

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
                            case EventTypeEnum.Klubture:
                            default:
                                item.Title = location.Name;
                                break;
                        }
                    }
                    context.Events.Add(item);
                }
            }


            await context.SaveChangesAsync();
        }

        public async Task CancelEventAsync(Event item)
        {
            item.IsCancelled = true;
            // TODO: Refund users
            context.Update(item);
            await context.SaveChangesAsync();
        }
        public async Task ActivateEventAsync(Event item)
        {
            item.IsCancelled = false;
            // TODO: Refund users
            context.Update(item);
            await context.SaveChangesAsync();
        }

    }
}
