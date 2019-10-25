using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    [Table("ApplicationUserAccountEntry")]
    public class ApplicationUserAccountEntry : ModelBase
    {

        [Display(Name ="Bogføringsdato", Description ="Dato for transaktionen")]
        [DataType(DataType.DateTime)]
        public DateTime? PostingDate { get; set; }

        [Display(Name ="Beskrivelse", Description ="Angiv en beskrivelse af trankationen")]
        [StringLength(250, ErrorMessage ="Beskrivelsen må kun være 250 tegn")]
        public string Description { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        [Range(1,double.MaxValue, ErrorMessage = "Beløbet skal mindst være 1")]
        [Display(Name ="Beløb", Description ="Det beløb der skal haæves eller indsættes på brugerens konto")]
        public decimal Amount { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Event Event { get; set; }

    }
}
