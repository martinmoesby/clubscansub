using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public CourseTypeEnum CourseType { get; set; }

        [Display(Name ="Sessioner i klasseværelse")]
        public int AcademicSessions => Sessions != null ? Sessions.Count(x => x.SessionType == CourseSessionTypeEnum.AcademicSession) : 0;

        [Display(Name = "Sessioner i svømmehal")]
        public int PoolSessions => Sessions != null ? Sessions.Count(x => x.SessionType == CourseSessionTypeEnum.PoolSession) : 0;

        [Display(Name = "Sessioner i åbent vand")]
        public int OpenWaterSessions => Sessions != null ? Sessions.Count(x => x.SessionType == CourseSessionTypeEnum.OpenWaterSession) : 0;
    
        public virtual ICollection<CourseSessionTemplate> Sessions { get; set; }

        ////public virtual Club Club { get; set; }
        //public virtual ICollection<Course> Courses { get; set; }
    }
}
