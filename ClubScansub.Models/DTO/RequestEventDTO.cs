using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Models.DTO
{
    public class RequestEventDTO
    {
        public string RequesterId { get; set; }
        public int DivelocationId { get; set; }
        public DateOnly[] EventDates { get; set; }
        public string[] Users { get; set; }

        public string Note { get; set; }

        public RequestEventDTO() { }

        public RequestEventDTO(string requesterId, int divelocationId, DateOnly[] eventDates, string[] users)
        {
            RequesterId = requesterId;
            DivelocationId = divelocationId;
            EventDates = eventDates;
            Users = users;
        }

    }
}
