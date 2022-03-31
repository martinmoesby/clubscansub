using ClubScansub.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{
    [Table("CourseSessionTemplate")]
    public class CourseSessionTemplate
    {
        public CourseSessionTemplate()
        {
        }
    
        public int Id { get; set; }

        [Display(Name="Session nr.")]
        public string SessionNumber { get; set; }
        [Display(Name="Session navn")]
        public string Name { get; set; }
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Display(Name="Sessionstype")]
        public CourseSessionTypeEnum SessionType { get; set; }

        [Display(Name="Standard starttidspunkt")]       
        public System.TimeSpan DefaultStartTime { get; set; }
        [Display(Name="Standard varighed")]
        public System.TimeSpan DefaultDuration { get; set; }

        [Display(Name ="Standard Ugedag")]
        public System.DayOfWeek DefaultWeekday { get; set; }

        [Display(Name = "Brug standard Ugedag")]
        public bool UseDefaultWeekDay { get; set; }

        [Display(Name = "Standard mødested")]
        public Address Address { get; set; }

        [Display(Name = "Instruktør-til-Elev ratio 1:X")]
        public int InstructorRatio { get; set; }

        public CourseTemplate CourseTemplate { get; set; }

    }
}
