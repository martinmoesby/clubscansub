using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{

    [Table("Diveorganization")]
    public class Diveorganization : BaseModel
    {

        [MaxLength(50, ErrorMessage ="Name cannot be longer than 50 chars")]
        [Display(Name = "Fulde navn")]
        public string Name { get; set; }

        [MaxLength(20,ErrorMessage ="Short name cannot be longer than 20 chars")]
        [Display(Name ="Kalde navn")]
        [Required]
        public string ShortName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DiveorgCertificate> Certificates { get; set;}
    }
}
