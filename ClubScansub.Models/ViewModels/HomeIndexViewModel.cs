using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models.ViewModels
{
    public class HomeIndexViewModel : BaseViewModel
    {
        public IEnumerable<Event> UpcomingEvents { get; set; }
        public string CalendarType { get; set; }

    }
}
