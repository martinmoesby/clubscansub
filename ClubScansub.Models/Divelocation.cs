using ClubScansub.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{
    [Table("Divelocation")]
    public class Divelocation : BaseModel
    {
        public Divelocation()
        {

        }

        [Required]
        [StringLength(150, ErrorMessage ="Navnet må max. være 150 karakterer")]
        [Display(Name="Dykkersted/Vragnavn")]
        public string Name { get; set; }
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Der skal angives en min. dybde")]
        [Range(1, int.MaxValue, ErrorMessage = "Min-dybde skal være over 1")]
        [Display(Name = "Min. dybde")]
        public int MinDepth { get; set; }

        [Required(ErrorMessage = "Der skal angives en max. dybde")]
        [Range(1,int.MaxValue,ErrorMessage ="Max-dybde skal være over 1")]
        [Display(Name = "Max. dybde")]
        public int MaxDepth { get; set; }

        [Display(Name = "Min. deltagere")]
        public int MinParticipants { get; set; }

        [Display(Name = "Max. deltagere")]
        public int MaxParticipants { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [Display(Name="Normal pris")]
        [Column(TypeName ="decimal")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Der skal angives dyk-type")]
        [Display(Name = "Dyk type")]
        public DiveTypeEnum DiveType { get; set; }
        
        //[Display(Name = "Billede")] 
        //public byte[] Image { get; set; }

        [Display(Name = "Mødested")]
        public Address MeetingLocation { get; set; }

        [Display(Name = "Min. certificering")]
        public Certificate Certificate { get; set; }

        [Display(Name = "Normal start tidspunkt")]
        public TimeSpan DefaultStartTime { get; set; } = new TimeSpan(8, 30, 0);


        [Display(Name = "Normal varighed")]
        public TimeSpan DefaultDuration { get; set; } = new TimeSpan(4, 0, 0);

        [Display(Name = "Antal Dage")]
        public int DefaultDays { get; set; } = 1;


        [Display(Name = "Tur type")] 
        public EventTypeEnum DefaultEventType { get; set; }
        
        [JsonIgnore]
        public ICollection<Event> Events { get; set; }

        [JsonIgnore]
        public virtual DivelocationImage Image { get; set; }
        //public virtual ICollection<CourseSession> CourseSessions { get; set; }
    }
}
