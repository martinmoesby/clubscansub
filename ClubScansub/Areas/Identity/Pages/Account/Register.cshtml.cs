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
            public string PhoneNumber { get; set; }

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

                var result = await _userManager.CreateAsync(user, randomPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Userroles.User);
                    
                    _logger.LogInformation("User created a new account with password.");

                    var mailMessage = Texts.WelcomeMail(Input.Firstname, Input.UserName) +
                        Texts.EventAccountTerms() +
                        Texts.DepositText() +
                        Texts.EventTerms() +
                        Texts.RegardsText();

                        //$"Tillykke. Så er din klubkonto hos Dykkerklubben Scansub blevet oprettet.<br/>" +
                        //$"Dit login er <h2>{Input.UserName}@scansub.dk</h2> og du kan nu logge på sitet og evt. tilknytte din facebook-konto. <br />" +
                        //$"Dit kodeord bliver sendt i en anden mail af sikkerhedsmæssige årsager. <br/>" +
                        //$"Husk at dit login er personligt og må ikke overdrages til andre. <br />" +
                        //$"Dit medlemskab er oprettet som et basis medlemsskab - se hvilke fordele du får på www.dkdiver.dk <br />" +
                        //$"<br />" +
                        //$"<br />" +
                        //$"Med venlig hilsen <br />Scansub DK Diver";

                    await _emailSender.SendEmailAsync(Input.Email, "Dit Basis medlemskab er blevet oprettet",mailMessage);

                    var passwordMessage = $"Hej {Input.Firstname}," +
                        $"Tillykke. Så er dit basis medlemsskab hos Dykkerklubben Scansub blevet oprettet. " +
                        $"Dit kodeord er '{randomPassword}' og du kan nu logge på sitet og evt. tilknytte din facebook-konto. " +
                        $"Husk at dit kodeord er personligt og må ikke overdrages til andre. " +
                        $"Du kan skifte dit kodeord under 'Min Konto' -> 'Kodeord' " +
                        $"" +
                        $"Med venlig hilsen " +
                        $"Scansub DK Diver";

                    await _smsSender.SendSmsAsync(Input.PhoneNumber, passwordMessage);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //if (!User.IsInRole(Userroles.Administrator) && !User.IsInRole(Userroles.Owner))
                    //    await _signInManager.SignInAsync(user, isPersistent: false);

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
