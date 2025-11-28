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
using System;
using System.Runtime.CompilerServices;

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
        //private readonly ApplicationDbContext context;
        private readonly EventUserService userService;
        public EventService(EventUserService userService, IOptions<ServiceOptions> options, IEmailSender emailSender) : base(options, emailSender)
        {
            //context = new ApplicationDbContext(dbContextOptions);
            this.userService = userService;
        }

        public Task<Event> AddAsync(Event Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Event>> AddAsync(IList<Event> Items)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
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
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Events.Remove(Item);
            await context.SaveChangesAsync();
        }

        public async Task<IList<Event>> GetActiveEventsByDivesiteId(int divesiteId)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Events.Include(x=>x.Participants).Where(x => x.StartDateAndTime > DateTime.UtcNow && x.Divelocation.Id == divesiteId);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Events.Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser).Where(x=>x.EventType != Utility.EventTypeEnum.NotAnEvent);
            return await data.ToListAsync();
        }

        public async Task<IList<Event>> GetAllByDateAsync(DateTime startDate, DateTime endDate)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.Events.Include(x=>x.Participants).Include(x=>x.ExternalClubMembers).Include(x=>x.Divelocation).Where(x => x.StartDateAndTime >= startDate && x.EndDateAndTime <= endDate && x.EventType != Utility.EventTypeEnum.NotAnEvent).ToListAsync<Event>();
            return data;
        }

        public async Task<IList<CourseSession>> GetAllCourseSessionsByDateAsync(DateTime startDate, DateTime endDate)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.CourseSessions.Include(x=>x.Course).ThenInclude(x=>x.Participants).Where(x=>x.DateTime >= startDate && x.DateTime <= endDate).ToListAsync();
            return data;
        }

        public async Task<Event> GetAsync(string Id)
        {
            var id = int.Parse(Id);

            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.Events.Include(x=>x.Participants).Include(x => x.Divelocation).ThenInclude(x=>x.Image).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data == null)
                throw new Exception($"Event with Id '{Id}' could nor be retrieved.");

            return data;

        }

        public async Task<Event> GetAsync(Guid Id)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.Events.Include(x => x.Participants).Include(x => x.Divelocation).ThenInclude(x => x.Image).Where(x => x.DeeplinkId == Id).FirstOrDefaultAsync();
            if (data == null)
                throw new Exception($"Event with Id '{Id}' could nor be retrieved.");

            return data;
        }

        public async Task<IList<Event>> GetCompletedEventsByDivesiteId(int divesiteId)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Events.Include(x => x.Participants).Where(x => x.StartDateAndTime < DateTime.UtcNow && x.Divelocation.Id == divesiteId);
            return await data.ToListAsync();
        }

        public async Task AddedUserToEvent(string userId, Event @event )
        {
            await userService.AddUserToEvent(userId, @event.Id);
        }

        public async Task<Event> UpdateAsync(Event item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
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
            using var context = new ApplicationDbContext(dbContextOptions);
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

        /// <summary>
        /// Creates a new event based on the specified event request and the selected proposed date.
        /// </summary>
        /// <remarks>The method marks the request as processed and approved, creates the event, adds
        /// participants, and sends a notification email to the requester. The event is scheduled using the selected
        /// proposed date and the default start time and duration from the dive location.</remarks>
        /// <param name="request">The event request containing details such as the dive location, requester, proposed dates, and additional
        /// participants. Cannot be null.</param>
        /// <param name="proposedDateIndex">The zero-based index of the proposed date to use from the request's list of dates. Must be a valid index
        /// within the range of available dates.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when the event has been created and
        /// related updates have been saved.</returns>
        /// <exception cref="Exception">Thrown if the dive location specified in the request cannot be found.</exception>
        public async Task<Event> CreateEventFromRequest(EventRequest request, DateOnly proposedDate)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var divelocation = await context.Divelocations
                .Include(x=>x.MeetingLocation)
                .Include(x=>x.Certificate)
                .Where(x => x.Id == request.Divelocation.Id)
                .FirstOrDefaultAsync();
            
            if (divelocation == null)
                throw new Exception($"Could not create event from request #{request.Id} because divelocation with Id '{request.Divelocation.Id}' could not be found.");

            var participants = await context.ApplicationUsers.Where(x => request.AdditionalParticipants.Contains(x.Id) || x.Id == request.Requester.Id).ToListAsync();

            var date = new DateTime(proposedDate,TimeOnly.FromTimeSpan(divelocation.DefaultStartTime));
            var endDate = date.AddHours(divelocation.DefaultDuration.Hours);

            var @event = new Event
            {
                Title = $"Forespurgt tur til '{divelocation.Name}' ",
                Details = request.Notes,
                EventType = divelocation.DefaultEventType,
                MinParticipants = divelocation.MinParticipants,
                MaxParticipants = divelocation.MaxParticipants,
                Price = divelocation.Price,
                PremiumPrice = divelocation.DefaultEventType == EventTypeEnum.Klubture ? 0 : divelocation.Price,
                StartDateAndTime = date,
                EndDateAndTime = endDate,
                Divelocation = divelocation,
                FixedParticipants = 0,
                //Participants = participants.Select(x => new EventUser { ApplicationUser = x }).ToList(),
                DeeplinkId = Guid.NewGuid(),
                Address = divelocation.MeetingLocation,
                RequiredCertificate = divelocation.Certificate,

            };

            await context.Events.AddAsync(@event);
            await context.SaveChangesAsync();

            await AddedUserToEvent(request.Requester.Id, @event);
            foreach (var participant in participants.Where(x => x.Id != request.Requester.Id))
            {
                await AddedUserToEvent(participant.Id, @event);
            }

            return @event;
        }

        /// <summary>
        /// Denies the specified event request and notifies the requester by email.
        /// </summary>
        /// <remarks>After the request is denied, an email notification is sent to the requester. The
        /// changes are saved to the database as part of this operation.</remarks>
        /// <param name="request">The event request to be denied. Cannot be null. The request's status will be updated to indicate it has been
        /// processed and denied.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DenyEventRequest(EventRequest request)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            request.RequestProcessed = true;
            request.RequestApproved = false;
            context.Update(request);
            await context.SaveChangesAsync();
            if (request.Requester?.Email != null)
            {
                await emailSender.SendEmailAsync(request.Requester.Email!, "Requast has been denied", $"Your request for a trip to {request.Divelocation.Name} has been denied. Please contact us for more information.");
            }

        }

        public async Task ApproveEventRequest(EventRequest request, Event @event)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            request.RequestProcessed = true;
            request.RequestApproved = true;
            context.Update(request);
            await context.SaveChangesAsync();
            if (request.Requester?.Email != null)
            {
                await emailSender.SendEmailAsync(request.Requester.Email!, "Requast has been approved", $"Your request for a trip to {request.Divelocation.Name} has been approved, and is scheduled to tkae place on {@event.StartDateAndTime.ToString()}");

            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>

        public async Task CancelEventAsync(Event item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            item.IsCancelled = true;
            // TODO: Refund users
            context.Update(item);
            await context.SaveChangesAsync();
            await userService.RefundForCancelledEvent(item);
        }

        /// <summary>
        /// Reactivates a previously cancelled event asynchronously.
        /// </summary>
        /// <remarks>Call this method to mark an event as active after it has been cancelled. Changes are
        /// persisted to the database upon completion of the operation.</remarks>
        /// <param name="item">The event to be reactivated. The event's IsCancelled property will be set to false.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ActivateEventAsync(Event item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            item.IsCancelled = false;
            // TODO: Refund users
            context.Update(item);
            await context.SaveChangesAsync();
        }
        public async Task<List<EventRequest>> GetEventRequestsForDivesite(int divelocationId)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            return await context.EventRequests
                .Include(x => x.Divelocation)
                .Include(x => x.Requester)
                .Include(x => x.Event)
                .Where(x => x.Divelocation.Id == divelocationId && !x.RequestProcessed)
                .ToListAsync();
        }
        public async Task<List<EventRequest>> GetEventRequestsAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.EventRequests
                .Include(x => x.Divelocation).ThenInclude(x=>x.Image)
                .Include(x => x.Requester)
                .Where(x => !x.RequestProcessed)
                .ToListAsync();
            return data;
        }
        public async Task<List<EventRequest>> UpdateEventRequestsAsync(EventRequest request)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.Update(request);
            await context.SaveChangesAsync();
            return await GetEventRequestsForDivesite(request.Divelocation.Id);
        }
    }
}
