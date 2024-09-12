using ClubScansub.Models.Interface;
using ClubScansub.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{

    [Table("Event")]
    public class Event : BaseModel, ICalendarEvent
    {
        public Event()
        {
            Participants = new Collection<EventUser>();
        }

        [Display(Name = "Navn")]
        public string Title { get; set; }

        [Display(Name = "Detaljer")]
        public string Details { get; set; }

        [Display(Name = "Tur type")]
        public EventTypeEnum EventType { get; set; }

        [Display(Name = "min. deltagere")]
        public int? MinParticipants { get; set; }

        [Display(Name = "Max. pladser")]
        public int? MaxParticipants { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal")]
        [Display(Name = "Pris")]
        public decimal? Price { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal")]
        [Display(Name = "Prim (Prem. medlem)")]
        public decimal? PremiumPrice { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal")]
        [Display(Name = "Depositum")]
        public decimal Deposit { get; set; }

        [Display(Name = "Starttidspunkt")]
        [DataType(DataType.DateTime)]
        public System.DateTime StartDateAndTime { get; set; }

        [Display(Name = "Sluttidspunkt")]
        [DataType(DataType.DateTime)]
        public System.DateTime EndDateAndTime { get; set; }

        [Display(Name = "Aflyst?")]
        public bool IsCancelled { get; set; }

        [Display(Name = "Spærrede pladser")]
        public int? FixedParticipants { get; set; }

        [Display(Name = "Kun for Instruktører?")]
        public bool IsInternal { get; set; }

        [Display(Name = "Gratis for Instruktører?")]
        public bool IsFreeForDivepros { get; set; }

        //[Display(Name = "Gratis for Premium?")]
        //public bool IsFreeForPremiumMembers { get; set; }

        public Certificate RequiredCertificate { get; set; }

        public Divelocation Divelocation { get; set; }

        public Divelocation? SecondaryDivelocation { get; set; }

        public Address Address { get; set; }

        [Display(Name = "Dykkerleder")]
        public ApplicationUser Diveleader { get; set; }

        [Display(Name = "Standby dykker")]
        public ApplicationUser StandbyDiver { get; set; }

        [Display(Name = "Bådfører")]
        public ApplicationUser BoatLeader { get; set; }

        public virtual ICollection<EventMultiApplicationUser> ExternalClubMembers { get; set; }

        public ICollection<EventUser> Participants { get; set; }

        public ICollection<ApplicationUserAccountEntry> AccountTransactions { get; set; }

        [NotMapped]
        [Display(Name = "Frie pladser")]
        public int? FreeSpots => MaxParticipants - FixedParticipants - Participants.Count - ExternalClubMembersCount;

        [NotMapped]
        [Display(Name = "Manglende deltagere")]
        public int? RequiredSpots => MinParticipants - Participants.Count - FixedParticipants - ExternalClubMembersCount < 0 ? 0 : MinParticipants - Participants.Count - FixedParticipants - ExternalClubMembersCount;

        public Guid DeeplinkId { get; set; }

        [NotMapped]
        private int ExternalClubMembersCount => ExternalClubMembers == null ? 0 : ExternalClubMembers.Count;

    }
}
