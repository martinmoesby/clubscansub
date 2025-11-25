using ClubScansub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface IEventService : IGenericService<Event>
    {
        Task<IList<Event>> GetActiveEventsByDivesiteId(int divesiteId);
        Task<IList<Event>> GetAllByDateAsync(DateTime startDate, DateTime endDate);
        Task<IList<Event>> GetCompletedEventsByDivesiteId(int divesiteId);
        Task<List<EventRequest>> GetEventRequestsAsync();
        Task<List<EventRequest>> GetEventRequestsForDivesite(int divelocationId);
    }
}
