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
                    AccountNumber= Input.UserName
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

    class Texts
    {
        public static string WelcomeMail(string name, string username)
        {
            var text = $"Hej {name}" +
                $"Du er blevet oprettet med et basis medlemskab og en turkonto i Dykkerklubben Scansub." +
                $"<br/>" +
                $"Dit medlemsnummer er: <strong>{username}</strong> <br/>" +
                $"<br/>" +
                $"Dit kodeord blive tilsendt på en SMS af sikkerhedsmæssige årsager, så hvis du ikke har opgivet et mobiltelefon i din registreringsformular, " +
                $"bedes du henvende dig i butikken eller på nedenstående telefonnummer.<br/>" +
                $"<<br/>" +
                $"Du kan ændre dit password ved at logge ind på www.scansub.dk og gå til min 'Min konto' i øverste højre hjørne.<br/>" +
                $"Her kan du så administrere kodeord, kontakoplysninger smat tilknytte en facebook-konto så du fremover kan logge på med denne. <br/>" +
                $"Husk at få verificeret dit tlf. nr. samt din mail adresse" +
                $"";
            return text;
        }

        public static string RegardsText()
        {
            return $"Med venlig hilsen <br />" +
            $"<br/>" +
            $"John Karlsen <br/>" +
            $"<br/>" +
            $"Scansub DK Diver<br/>" +
            $"Industrivej 51F<br/>" +
            $"4000 Roskilde<br/>" +
            $"Tlf. + 45 46 75 05 75<br/>" +
            $"e - mail            info@dkdiver.dk<br/>" +
            $"web               www.dkdiver.dk<br/>" +
            $"<br/>" +
            $"Forretningens åbningstider:<br/>" +
            $"Man - Tors      Kl.  09.00 - 18.00<br/>" +
            $"Fredag           Kl.  09.00 - 18.00<br/>" +
            $"Lørdag           Kl.  09.00 - 15.00<br/>" +
            $"";

        }

        public static string DepositText()
        {
            return $"<br/>" +
            $"Hvis du ønsker at indsætte penge på din Turkonto, kan det gøres på følgende 3 måder:<br/>" +
            $"<br/>" +
            $"1.<br/>" +
            $"Via Webbank til vores bankkonto i Sparekassen Sjælland-Fyn:<br/>" +
            $"Reg.nr. 0520 - Konto nr. 727989<br/>" +
            $"opgiv blot dit navn og debitornummer så indsætter vi beløbet på din Turkonto.<br/>" +
            $"<br/>" +
            $"2.<br/>" +
            $"Eller du kan indsætte penge på din konto ved at henvende dig i vores forretning.<br/>" +
            $"<br/>" +
            $"3.<br/>" +
            $"Eller du kan gå ind i vores internet butik<br/>" +
            $"- under ture, rejser og klub<br/>" +
            $"- vælg varen der hedder indsæt på Turkonto til 1 kr.<br/>" +
            $"- Husk at rette varen, til det stk.antal du ønsker at indsætte på Turkontoen<br/>" +
            $"- eller vælg inde på varen hvilket beløb du ønsker at indsætte<br/>" +
            $"-Put varen i indkøbskurven og gå til dankort betaling<br/>" +
            $"<br/>";

        }

        public static string EventAccountTerms()
        {
            return $"<ul><li>Turkontoen kan <strong>kun</strong> anvendes i forbindelse med tilmelding til aktiviter.</li>" +
            $"<li>Turkontoen kan <strong>IKKE</strong> anvendes til kursus tilmelding, tilmelding til rejser eller vare køb i butikken!</li>" +
            $"<li>Det er <strong>IKKE</strong> muligt at få refunderet indbetalinger på tur kontoen, de kan kun anvendes til dykker ture!</li></ul>" +
            $"<br/>" +
            $"<br/>" +
            $"Ved tilmelding til aktiviteter trækkes tur gebyret automatisk fra din Turkonto, det " +
            $"vil derfor kun være muligt at tilmelde sig ture hvis saldoen på din Turkonto " +
            $"som minimum modsvarer aktivitetsgebyret.<br/>" +
            $"<br/>";
        }

        public static string EventTerms()
        {
            return $"Du kan tilmelde dig aktiviteter online ved at logge på din konto på www.scansub.dk<br/>" +
            $"Du kan kun tilmelde dig aktiviteter, hvis du har dækning på din turkonto.<br/>" +
            $"<br/>" +
            $"Under menupunktet 'Klubmedlem' kan du følge med i dine tilmeldinger og status på de aktiviteter, du har tilmeldt dig.<br/>" +
            $"Under 'Min konto' kan du se din turkonto samt vedligeholde dine medlemsdata, certifikater, ændre password m.m.<br/>" +
            $"<br/>" +
            $"Bemærk venligst!<br/>" +
            $"Du kan sagtens afmelde dig fra en aktivitet, men vi har følgende regler:<br/>" +
            $"<br/>" +
            $"<ol><li>Hvis du afmelder dig mere end 7 dage før afvikling af aktiviteten refunderer vi det fulde beløb til din turkonto.</li>" +
            $"<li>Afmelder du dig mellem 7 dage og 24 timer før, refunderer vi kun halvdelen til din turkonto.</li>" +
            $"<li>Hvis din afmelding sker senere end 24 timer før, refunderer vi intet.</li></ol>" +
            $"<br/>" +
            $"Udlandsrejser, samt flerdags aktiviteter har særlige regler for tilmelding og afmelding " +
            $"som vil være beskrevet under den enkelte aktivitet.<br/>" +
            $"<br/>" +
            $"Afmelding fra disse kan ske ved at ringe på tlf. 46 75 05 75 " +
            $"eller skrive mail til info@dkdiver.dk<br/>" +
            $"<br/>";

        }
    }
}
