using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    public class DiveorgCertificate
    {
        [Key]
        public int DiveorganizationId { get; set; }
        [Key]
        public int CertificateId { get; set; }
        public Diveorganization Diveorg { get; set; }
        public Certificate Certificate { get; set; }
    }
}
