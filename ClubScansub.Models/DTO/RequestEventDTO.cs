using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Models.DTO
{
    public class RequestEventDTO
    {
        public int DivelocationId { get; set; }
        public DateOnly[] EventDates { get; set; }
        public string[] Users { get; set; }

        public RequestEventDTO() { }

        public RequestEventDTO(int divelocationId, DateOnly[] eventDates, string[] users)
        {
            DivelocationId = divelocationId;
            EventDates = eventDates;
            Users = users;
        }

    }
}
