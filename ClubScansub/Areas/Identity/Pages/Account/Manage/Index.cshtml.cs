using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender smsSender;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext db,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            this.db = db;
            this.smsSender = smsSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
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
            [PersonalData]
            [Display(Name = "Debitor nummer")]
            public string AccountNumber { get; set; } 
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var appUser = await db.ApplicationUsers.FindAsync(user.Id);
            //var userName = await _userManager.GetUserNameAsync(user);
            //var email = await _userManager.GetEmailAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //Username = userName;

            Input = new InputModel
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                PhoneNumber =appUser.PhoneNumber,
                Firstname = appUser.Firstname,
                Lastname = appUser.Lastname,
                Streetaddress = appUser.Streetaddress,
                PostalCode = appUser.PostalCode,
                City = appUser.City,
                Country = appUser.Country,
                DayOfbirth = appUser.DayOfbirth,
                AccountNumber = appUser.AccountNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            IsPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var appUser = await db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == Input.UserName);

            var email = await _userManager.GetEmailAsync(appUser);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(appUser, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(appUser);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(appUser);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(appUser, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(appUser);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            appUser.Firstname = Input.Firstname;
            appUser.Lastname = Input.Lastname;
            appUser.Streetaddress = Input.Streetaddress;
            appUser.PostalCode = Input.PostalCode;
            appUser.City = Input.City;
            appUser.Country = Input.Country;
            appUser.DayOfbirth = Input.DayOfbirth;
            appUser.PhoneNumber = Input.PhoneNumber;
            appUser.AccountNumber = Input.AccountNumber;

            await db.SaveChangesAsync();

            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}




            await _signInManager.RefreshSignInAsync(appUser);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Verificer din mail-adresse",
                $"Venligst <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klik her</a> for at verificere din email.");

            StatusMessage = "Verificerings mail afsendt. Tjek din e-mail.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendPhonenumberVerificationAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var phoneNumber = Regex.Replace(await _userManager.GetPhoneNumberAsync(user),@"\s+","");

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            await smsSender.SendSmsAsync(phoneNumber, $"Bruge denne kode til at verificere dit telefonenummer: {code}");

            StatusMessage = "SMS sendt til din telefon.";

            return RedirectToPage("/Account/ConfirmNumber");
        }
    }
}
