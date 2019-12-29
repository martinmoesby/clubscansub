using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClubScansub.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

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

            public string PhotoUrl { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {

                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                ErrorMessage = "Fejl: Du har ikke tilknyttet Ekstern login til din konto. Login med brugernavn /kodeord, og gå til 'Min konto' -> 'Eksterne konti' for at tilknytte Eksternt login til din konto";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser
            //    {
            //        UserName = Input.Email,
            //        Email = Input.Email,
            //        Streetaddress = Input.Streetaddress,
            //        PhotoUrl = Input.PhotoUrl,
            //        PostalCode = Input.PostalCode,
            //        Firstname = Input.Firstname,
            //        City = Input.City,
            //        Country = Input.Country,
            //        PhoneNumber = Input.PhoneNumber,
            //        DayOfbirth = Input.DayOfbirth
            //    };
                
            //    var result = await _userManager.CreateAsync(user);
            //    if (result.Succeeded)
            //    {

            //        await _userManager.AddToRoleAsync(user, Userroles.User);

            //        result = await _userManager.AddLoginAsync(user, info);
            //        if (result.Succeeded)
            //        {
            //            await _signInManager.SignInAsync(user, isPersistent: false);
            //            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
            //            return LocalRedirect(returnUrl);
            //        }
            //    }
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //}

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
