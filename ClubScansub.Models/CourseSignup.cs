using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{
    [Table("CourseSignup")]
    public class CourseSignup
    {
        [Key]
        public string ApplicationUserId { get; set; }
        [Key]
        public int CourseId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Course Course { get; set; }
    }
}
