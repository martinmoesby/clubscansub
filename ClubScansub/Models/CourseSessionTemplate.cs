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
        public WeekdayEnum DefaultWeekday { get; set; }

        public virtual CourseTemplate CourseTemplate { get; set; }

    }
}
