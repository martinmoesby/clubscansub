using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubScansub.Models
{ 
    [Table("Address")]
    public class Address : BaseModel
    {
        public Address()
        {
        }

        [StringLength(50, ErrorMessage ="Navn må max være 50 tegn")]
        [Display(Name="Stednavn")]
        //[Required(ErrorMessage ="Der skal angives et stednavn")]
        public string Name { get; set; }

        [StringLength(150, ErrorMessage ="Vejnavn kan max være 150 tegn")]
        [Display(Name = "Vejnavn og nr.")]    
        public string Streetname { get; set; }

        [StringLength(10, ErrorMessage = "Postnr kan max være 4 tegn")]
        [Display(Name = "Postnr.")]
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [StringLength(50, ErrorMessage = "Bynavn kan max være 50 tegn")]
        [Display(Name = "By")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "Land kan max være 50 tegn")]
        [Display(Name = "Land")]
        public string Country { get; set; }

        //public virtual Club Club { get; set; }

        public virtual ICollection<Divelocation> Divelocations { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        [Display(Name = "Breddegrad")]
        public string Latitude { get; set; }
        [Display(Name = "Længdegrad")] 
        public string Longitude { get; set; }

        //public virtual ICollection<CourseSession> CourseSessions { get; set; }

    }
}
