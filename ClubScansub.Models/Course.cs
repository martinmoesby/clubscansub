using ClubScansub.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{
    
    public class Course : Event
    {
        public Course()
        {
            Signups = new Collection<CourseSignup>();
        }
    
        public string CourseName { get; set; }
        public CourseTypeEnum CourseType { get; set; }
        public virtual ICollection<CourseSession> CourseSessions { get; set; }
        public virtual CourseTemplate CourseTemplate { get; set; }

        public virtual ICollection<CourseSignup> Signups { get; set; }


        [NotMapped]
        [Display(Name = "Frie pladser")]
        new public int FreeSpots => MaxParticipants - FixedParticipants - Participants.Count - Signups.Count;

        [NotMapped]
        [Display(Name = "Manglende deltagere")]
        new public int RequiredSpots => MinParticipants - Participants.Count - FixedParticipants - Signups.Count < 0 ? 0 : MinParticipants - Participants.Count - FixedParticipants - Signups.Count;


        [NotMapped]
        public bool InstructorsCovered
        {
            get
            {
                if (CourseSessions != null)
                {
                    foreach (var item in CourseSessions)
                    {
                        if (item.InstructorsMissing > 0)
                            return false;
                    }
                }
                return true;
            }
        }

        //public int? PrimaryInstructorId { get; set; }
        //public virtual Instructor PrimaryInstructor { get; set; }
        //public virtual ICollection<Instructor> SupplementalInstructors { get; set; }
        //public virtual ICollection<ApplicationUser> Students { get; set; }
    }
}
