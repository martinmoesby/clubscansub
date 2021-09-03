using ClubScansub.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{

    [Table("CourseTemplate")]
    public class CourseTemplate
    {
        public CourseTemplate()
        {
        }
    
        public int Id { get; set; }
        [Display(Name = "Navn")]
        public string TemplateName { get; set; }
        [Display(Name ="Standard varighed (uger)")]
        public string TemplateDurationInWeeks { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName ="decimal")]
        [Display(Name = "Standard Pris")]
        public decimal DefaultPrice { get; set; }
        public CourseTypeEnum CourseType { get; set; }
        public ICollection<CourseSessionTemplate> Sessions { get; set; }
        
        [Display(Name ="Sessioner i klasseværelse")]        
        public int AcademicSessions { get; set; }
        
        [Display(Name = "Sessioner i svømmehal")]
        public int PoolSessions { get; set; }
        
        [Display(Name = "Sessioner i åbent vand")]
        public int OpenWaterSessions { get; set; }
        
        [Display(Name = "Min. antal deltagere")]
        public int MinStudents { get; set; }

        [Display(Name = "Max. antal deltagere")]
        public int MaxStudents { get; set; }
        
        [Display(Name = "Billede")]
        public byte[] Image { get; set; }

        [Display(Name = "Krævet Instruktør certifikat")]
        public Certificate InstructorCertificate { get; set; }
        ////public virtual Club Club { get; set; }
        //public virtual ICollection<Course> Courses { get; set; }
    }
}
