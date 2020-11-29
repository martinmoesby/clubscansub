using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Medlemsnummer")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            //[Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "Password")]
            //public string Password { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            [Display(Name ="Mobiltelefon")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Du skal angive en bruger rolle")]
            [Display(Name = "Bruger type")]
            public string CreateAsUserRole { get; set; }


            [PersonalData]
            [Display(Name = "Fornavn")]
            public string Firstname { get; set; }

            [PersonalData]
            [Display(Name = "Efternavn")]
            public string Lastname { get; set; }

            [PersonalData]
            [DataType(DataType.Date)]
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
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = $"{Input.UserName}@scansub.dk",
                    Email = Input.Email,
                    Firstname = Input.Firstname,
                    Lastname = Input.Lastname,
                    Streetaddress = Input.Streetaddress,
                    PostalCode = Input.PostalCode,
                    DayOfbirth = Input.DayOfbirth,
                    City = Input.City,
                    PhoneNumber = Input.PhoneNumber,
                    AccountNumber = Input.UserName,
                    OldAccountImported = true                    
                };

                var randomPassword = RandomGenerator.GenerateString(8);
                string passwordMessage = "";
                string mailMessage = "";

                var result = await _userManager.CreateAsync(user, randomPassword);

                if (result.Succeeded)
                {
                    if (Input.CreateAsUserRole == Userroles.Student)
                    {
                        await _userManager.AddToRoleAsync(user, Userroles.Student);

                        passwordMessage = $"Hej {Input.Firstname}," +
                            $"Tillykke. Så er du blevet som kursist hos Dykkerklubben Scansub. " +
                            $"Dit kodeord er '{randomPassword}' og du kan nu logge på sitet og evt. tilknytte din facebook-konto. " +
                            $"Husk at dit kodeord er personligt og må ikke overdrages til andre. " +
                            $"Du kan skifte dit kodeord under 'Min Konto' -> 'Kodeord' " +
                            $"" +
                            $"Med venlig hilsen " +
                            $"Scansub DK Diver";

                        mailMessage = Texts.StudentWelcomeMail(Input.Firstname, Input.UserName) +
                            Texts.StudentTerms() +
                            Texts.RegardsText();
                    }

                    if (Input.CreateAsUserRole == Userroles.User)
                    {
                        await _userManager.AddToRoleAsync(user, Userroles.User);
                        passwordMessage = $"Hej {Input.Firstname}," +
                            $"Tillykke. Så er dit basis medlemsskab hos Dykkerklubben Scansub blevet oprettet. " +
                            $"Dit kodeord er '{randomPassword}' og du kan nu logge på sitet og evt. tilknytte din facebook-konto. " +
                            $"Husk at dit kodeord er personligt og må ikke overdrages til andre. " +
                            $"Du kan skifte dit kodeord under 'Min Konto' -> 'Kodeord' " +
                            $"" +
                            $"Med venlig hilsen " +
                            $"Scansub DK Diver";
                        mailMessage = Texts.WelcomeMail(Input.Firstname, Input.UserName) +
                            Texts.EventAccountTerms() +
                            Texts.DepositText() +
                            Texts.EventTerms() +
                            Texts.RegardsText();
                    }

                    if (Input.CreateAsUserRole == Userroles.Member)
                    {
                        await _userManager.AddToRoleAsync(user, Userroles.User);
                        await _userManager.AddToRoleAsync(user, Userroles.Member);
                        passwordMessage = $"Hej {Input.Firstname}," +
                            $"Tillykke. Så er dit premium medlemsskab hos Dykkerklubben Scansub blevet oprettet. " +
                            $"Dit kodeord er '{randomPassword}' og du kan nu logge på sitet og evt. tilknytte din facebook-konto. " +
                            $"Husk at dit kodeord er personligt og må ikke overdrages til andre. " +
                            $"Du kan skifte dit kodeord under 'Min Konto' -> 'Kodeord' " +
                            $"" +
                            $"Med venlig hilsen " +
                            $"Scansub DK Diver";
                        mailMessage = Texts.WelcomeMail(Input.Firstname, Input.UserName) +
                            Texts.EventAccountTerms() +
                            Texts.DepositText() +
                            Texts.EventTerms() +
                            Texts.RegardsText();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    await _emailSender.SendEmailAsync(Input.Email, "Din brugerkonto er blevet oprettet",mailMessage);

                    await _smsSender.SendSmsAsync(Input.PhoneNumber, passwordMessage);

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }


    class RandomGenerator
    {
        private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateString(int length)
        {
            var bytes = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
        }
    }


}
