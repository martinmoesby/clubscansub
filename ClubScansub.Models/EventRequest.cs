using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    public class EventRequest : BaseModel
    {
        public EventRequest() { }
        public List<DateOnly> Dates { get; set; }
        public Divelocation Divelocation { get; set; }
        public ApplicationUser Requester { get; set; }
        public string Notes { get; set; }
        public string AdditionalParticipants { get; set; }  

    }
}
