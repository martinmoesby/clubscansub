using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    public class ApplicationUser : IdentityUser
    {
        // TODO : Add specific user information - mark with [PersonalData] for GDPR support
        [PersonalData]
        [Display(Name = "Fornavn")]
        public string Firstname { get; set; }

        [PersonalData]
        [Display(Name = "Efternavn")]
        public string Lastname { get; set; }

        [PersonalData]
        [Display(Name = "Fødselsdato")]
        public DateTime DayOfbirth { get; set; }

        [PersonalData]
        [Display(Name = "Vejnavn og nr.")]
        public string Streetaddress { get; set; }
        [PersonalData]
        [Display(Name = "By")]
        public string City { get; set; }
        [PersonalData]
        [Display(Name = "Land")]
        public string Country { get; set; }
        [PersonalData]
        [Display(Name = "Postnr.")]
        public string PostalCode { get; set; }

        [PersonalData]
        public string PhotoUrl { get; set; }

        [PersonalData]
        [Display(Name = "Bruger nummer")]
        public string AccountNumber { get; set; }

        public string Name => $"{Firstname} {Lastname}";

        [NotMapped]
        [Display(Name = "Initialer")]
        public string Initials => string.Join(string.Empty, Firstname.Split(' ').Select(x => x[0]).Concat(Lastname.Split(' ').Select(x => x[0])).ToArray());
        //{
        //    get {
        //        var initials = string.Join(string.Empty,Firstname.Split(' ').Select(x => x[0]).Concat(Lastname.Split(' ').Select(x => x[0])).ToArray());
        //        return initials;
        //    }
        //}
        //$"{Firstname.Split(' ').ToList().ForEach(x => x[0]) } {Lastname?.Take(1).ToString().ToUpper()}";


        [PersonalData]
        [Display(Name ="Kursuskonto balance")]
        public decimal CourseAccountBalance => AccountTransactions == null ? 0 : AccountTransactions.Where(x=>x.AccountType == Utility.AccountTypeEnum.CourseAccountType).Sum(x => x.Amount);

        [PersonalData]
        [Display(Name = "Turkonto balance")]
        public decimal Balance => AccountTransactions == null ? 0 : AccountTransactions.Where(x => x.AccountType == Utility.AccountTypeEnum.EventAccountType).Sum(x => x.Amount);


        [PersonalData]
        [Display(Name = "Certifikater")]
        public virtual ICollection<UserCertificat> Certificates { get; set; }

        public virtual ICollection<EventUser> Events { get; set; }

        public virtual ICollection<CourseSignup> Courses { get; set; }

        public virtual ICollection<ApplicationUserAccountEntry> AccountTransactions { get; set; }

        public virtual ICollection<CourseSessionInstructor> InstructorSessions { get; set; }


    }

}
