using ClubScansub.Data;
using ClubScansub.Data.Migrations;
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
    /// <summary>
    /// Provides functionality for managing events, including creation, retrieval, updating, and deletion of events.
    /// </summary>
    /// <remarks>This service supports operations for handling events, such as retrieving events by date or
    /// location,  managing participants, and activating or canceling events. It also includes methods for working with 
    /// course sessions and creating events based on predefined configurations.</remarks>
    public class EventService : ServiceBase, IEventService
    {
        private readonly ApplicationDbContext context;
        private readonly EventUserService userService;
        public EventService(EventUserService userService, IOptions<ServiceOptions> options, IEmailSender emailSender) : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
            this.userService = userService;
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

        public async Task AddedUserToEvent(string userId, Event @event )
        {
            await userService.AddUserToEvent(userId, @event.Id);
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

        /// <summary>
        /// Creates and saves new events asynchronously based on the provided list of event templates.
        /// </summary>
        /// <remarks>This method processes each event in the provided list by retrieving the associated
        /// dive location and its related data (e.g., certificate, meeting location). It then creates a new event with
        /// properties derived from the template and the dive location's defaults. The new events are added to the
        /// database and saved asynchronously.  If the event's price is not specified (i.e., zero), the dive location's
        /// default price is used. Titles for the events are generated based on the event type and dive location if not
        /// explicitly provided.</remarks>
        /// <param name="events">A list of <see cref="Event"/> objects representing the event templates to be processed and created. Each
        /// event in the list must have a valid associated dive location.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when all events have been created and
        /// saved to the database.</returns>
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
            await userService.RefundForCancelledEvent(item);
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
