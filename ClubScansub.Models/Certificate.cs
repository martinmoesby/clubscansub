using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    [Table("Certificate")]
    public class Certificate : BaseModel
    {
        [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 chars")]
        [Display(Name = "Fulde navn")]
        public string Name { get; set; }

        [MaxLength(20, ErrorMessage = "Short name cannot be longer than 20 chars")]
        [Display(Name = "Kalde Navn")]
        public string ShortName { get; set; }

        [Display(Name ="Beskrivelse")]
        public string Description { get; set; }

        [Display(Name ="Maks. dybde")]
        public int? DepthLimit { get; set; }

        [Display(Name = "Pro certifikat?")]
        public bool IsDiveproCertificate { get; set; }

        public ICollection<DiveorgCertificate> Diveorgs { get; set; }

        [JsonIgnore]
        public ICollection<UserCertificat> UserCertificate { get; set; }
    }
}
