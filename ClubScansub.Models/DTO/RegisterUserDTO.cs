using ClubScansub.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ClubScansub.Models.DTO
{
    public class RegisterUserDTO : PasswordDTO, IAdress
    {        /// <summary>
             ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
             ///     directly from your code. This API may change or be removed in future releases.
             /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        [Display(Name = "Firstname", Description = "Enter your firstname(s)")]
        public string Firstname { get; set; } = "";

        //[Required]
        [Display(Name = "Lastname", Description = "Enter your Lastname/familyname")]
        public string Lastname { get; set; } = "";

        [Display(Name = "Nickname", Description = "The name you are most known by, if any..")]
        public string Nickname { get; set; } = "";
        //[Required]
        [Display(Name = "City")]
        public string City { get; set; } = "";
        //[Required] 
        public string Country { get; set; } = "";
        //[Required] 
        public string PostalCode { get; set; } = "";
        public string State { get; set; } = "";

        [Required]
        public string PhoneNumber { get; set; } = "";

        //[Required]
        [Display(Name = "Streetname and number")]
        public string Street { get; set; } = "";

    }
}
