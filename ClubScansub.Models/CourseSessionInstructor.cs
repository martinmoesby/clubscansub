using ClubScansub.Utility.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{

    [Table("CourseSessionInstructor")]
    public class CourseSessionInstructor //: BaseModel
    {

        [Key]
        public int CourseSessionId { get; set; }

        [Key]
        public string InstructorId { get; set; }

        [ForeignKey("CourseSessionId")]
        public CourseSession CourseSession { get; set; }

        [ForeignKey("InstructorId")]
        public ApplicationUser Instructor { get; set; }

        public bool InstructorApproved { get; set; }

        public bool InstructorRetracted { get; set; }

        [NotMapped]
        public SessionInstructorStatusEnum InstructorStatus{ 
            get 
            { 
                if (InstructorApproved) return SessionInstructorStatusEnum.Approved;
                if (InstructorRetracted) return SessionInstructorStatusEnum.Retracted;
                return SessionInstructorStatusEnum.SignedUp;

            } 
        }

    }
}
