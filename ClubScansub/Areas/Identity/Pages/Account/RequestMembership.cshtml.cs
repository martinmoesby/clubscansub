using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClubScansub.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RequestMembershipModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestMembershipModel(
            IEmailSender emailSender, ISmsSender smsSender, UserManager<IdentityUser> userManager)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _userManager = userManager;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Du skal angive din email adresse")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage ="Du skal angive Mobil nummer")]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage ="Du skal angive dit fornavn")]
            [Display(Name = "Fornavn")]
            public string Firstname { get; set; }

            [Display(Name = "Efternavn")]
            public string Lastname { get; set; }

            [Required(ErrorMessage ="Du skal angive din fødselsdato")]
            [DataType(DataType.Date)]
            [Display(Name = "Fødselsdato")]
            public DateTime DayOfbirth { get; set; }

            [Range(12,100, ErrorMessage ="Du skal angive et certifikat")]
            [Display(Name="Max certifikat (dybde)")]
            public string Certificate { get; set; }

            [Display(Name = "Vejnavn og nr.")]
            public string Streetaddress { get; set; }

            [Display(Name = "Postnr.")]
            public string PostalCode { get; set; }            
            
            [Display(Name = "By")]
            public string City { get; set; }

            [Display(Name = "Land")]
            public string Country { get; set; }

            [Display(Name ="Send en kopi til mig")]
            public bool WithCopyToSelf { get; set; }

        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
            {

                var formMessage = $"E-mail: {Input.Email}" + "<br/>" +
                    $"Fornavn: {Input.Firstname}" + "<br/>" +
                    $"Efternavn: {Input.Lastname}" + "<br/>" +
                    $"Adresse: {Input.Streetaddress}" + "<br/>" +
                    $"Postnr./By: {Input.PostalCode} {Input.City}" + "<br/>" +
                    $"Land : {Input.Country}" + "<br/>" +
                    $"Fødselsdag: {Input.DayOfbirth}" + "<br/>" +
                    $"Certifkat til: {Input.Certificate} m." + "<br/>" +
                    $"Mobil tlf.: {Input.PhoneNumber}" + "<br/>";

                var mailMesage = $"Hej Klub, <br/>" +
                    $"Så er der kommet en ny anmodning om medlemskab fra følgende bruger :<br/><br/>" +
                    $"{formMessage}" +
                    $"<br/><br/>Skynd jer at oprette ham i systemet, så han kan komme til at booke kurser og turer i vore gode klub" +
                    $"<br/><br/>Med venlig hilsen " +
                    $"<br/>Vores Klubkalender";

                var admins = await _userManager.GetUsersInRoleAsync(Userroles.Administrator);
                await _smsSender.SendMultipleSmsAsync(admins, mailMesage.Replace("<br/>","\n"));

                foreach (var admin in admins)
                {
                    await _emailSender.SendEmailAsync(admin.Email, "Ny medlemsforespørgsel", mailMesage);
                }

                if (Input.WithCopyToSelf )
                    await _emailSender.SendEmailAsync(Input.Email, "Kopi af Ny medlemsforespørgsel", mailMesage);

                return RedirectToPage("./RequestSend");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
