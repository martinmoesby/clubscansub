using ClubScansub.Utility;
using System;
using System.Collections.Generic;

namespace ClubScansub.Models.Interface
{
    public interface ICalendarEvent
    {
        int Id { get; set; }
        string Title { get;  }
        string Details { get; }
        decimal? Price { get;  }
        decimal? PremiumPrice { get; }
        int? MinParticipants { get; }
        int? MaxParticipants { get; }
        int? FixedParticipants { get; }

        DateTime EndDateAndTime { get; }
        DateTime StartDateAndTime { get; }

        ICollection<EventUser> Participants { get; }
        Divelocation Divelocation { get; }

        int? RequiredSpots { get; }
        int? FreeSpots { get; }
    }
}