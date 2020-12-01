using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    [Table("UserCertificate")]
    public class UserCertificat : BaseModel
    {
        public byte[] FrontSideImage { get; set; }

        public byte[] BackSideImage { get; set; }

        public DateTime IssuedDate { get; set; }

        public bool IsVerified { get; set; }

        public DateTime VerifiedDate { get; set; }

        public virtual ApplicationUser VerifiedBy { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Certificate Certificate { get; set; }

    }
}
