//------------------------------------------------------------------------------
using ClubScansub.Models.Interface;
using ClubScansub.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ClubScansub.Models
{

    public class CourseSession : BaseModel, ICalendarEvent
    {
        [Display(Name = "Dato og tidspunkt")]
        public DateTime DateTime { get; set; }
        
        [Display(Name ="Varighed (ca.)")]
        public TimeSpan Duration { get; set; }
        public CourseSessionTypeEnum Sessiontype { get; set; }
        
        [Display(Name = "Kursusdag - navn")]
        public string SessionName { get; set; }
        
        [Display(Name = "Kursusdag - beskrivelse")]
        public string SessionDescription { get; set; }
        public Course Course { get; set; }
        public Address Address { get; set; }
        public Divelocation Divelocation { get; set; }
        public CourseSessionTemplate CourseSessionTemplate { get; set; }

        [NotMapped]
        public int InstructorsRequired
        {
            get
            {
                if (Course?.Participants != null)
                {
                    var students = Course.Participants.Count;

                    if (CourseSessionTemplate?.InstructorRatio > 0)
                        return (students / CourseSessionTemplate.InstructorRatio) + 1;

                    switch (Sessiontype)
                    {
                        case CourseSessionTypeEnum.AcademicSession:
                            return (students / 12) + 1;

                        case CourseSessionTypeEnum.PoolSession:
                            return (students / 4) + 1;

                        case CourseSessionTypeEnum.OpenWaterSession:
                            return (students / 2) + 1;

                        default:
                            return 1;
                    }

                }
                else
                    return 0;
            }
        }

        [NotMapped]
        public int InstructorsMissing
        {
            get
            {
                return InstructorsRequired - (SessionInstructors != null ? SessionInstructors.Count() : 0); //x => x.InstructorApproved == true);
            }
        }

        public ICollection<CourseSessionInstructor> SessionInstructors { get; set; }

        [NotMapped]
        public string Title { get => Course?.CourseName; }
        [NotMapped] 
        public string Details { get => Course?.Details; }
        [NotMapped] 
        public decimal? Price { get => Course?.Price; }
        [NotMapped] 
        public decimal? PremiumPrice { get => Course?.PremiumPrice ; }
        [NotMapped] 
        public int? MinParticipants { get => Course?.MinParticipants; }
        [NotMapped] 
        public int? MaxParticipants { get => Course?.MaxParticipants; }
        [NotMapped] 
        public int? FixedParticipants { get => Course?.FixedParticipants; }
        [NotMapped] 
        public DateTime EndDateAndTime { get => DateTime.Add(Duration); }
        [NotMapped] 
        public DateTime StartDateAndTime { get => DateTime; }
        [NotMapped] 
        public ICollection<EventUser> Participants { get => Course!.Participants; }

        [NotMapped] 
        public int? RequiredSpots => Course?.RequiredSpots;

        [NotMapped] 
        public int? FreeSpots => Course?.FreeSpots;
    }
}
