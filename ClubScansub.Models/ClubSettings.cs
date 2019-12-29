using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClubScansub.Models
{
    [Table("Club")]
    public class ClubSettings : BaseModel
    {
        [Display(Name = "Hvad er klubbens navn?")]
        public string Name { get; set; }
        [Display(Name = "Hvad er klubbens tag-line?")]
        public string Tagline { get; set; }
        
        [Display(Name="Vælg logo")]
        public byte[] Logo { get; set; }

        [Display(Name = "Skriv dine Betingelser for brug af sitet")]
        public string TermsText { get; set; }
        [Display(Name = "Skriv dine privatlivsbetingelser")]
        public string PrivacyText { get; set; }

        [Display(Name = "Hvad er klubbens adresse=")]
        public virtual Address Address { get; set; }

        public bool IsOldMemberDatabaseImported { get; set; }

    }
}
