using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClubScansub.Models
{
    public class DivelocationImage: BaseModel
    {

        public Divelocation Divelocation { get; set; }
        public int DivelocationId { get; set; }

        [Display(Name = "Billede")]
        public byte[] ImageData { get; set; }
    }
}
