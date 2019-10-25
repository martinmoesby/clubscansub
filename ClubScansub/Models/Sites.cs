using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    [Table("Site")]
    public class Site : ModelBase
    {
        [Required]
        [MaxLength(50,ErrorMessage ="Name cannot be longer than 50 chars")]
        public string Name { get; set; }
        public string Description { get; set; }

        public string Adress { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public float Longitude { get; set; }
        public float Latitude { get; set; }

        [Required]
        public int MaxDivers { get; set; }

    }
}
