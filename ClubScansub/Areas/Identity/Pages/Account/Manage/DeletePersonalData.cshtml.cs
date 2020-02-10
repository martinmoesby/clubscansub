using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private IEmailSender _emailSender;
        private ISmsSender _smsSender;

        public DeletePersonalDataModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _smsSender = smsSender;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name ="Ja tak, jeg vil gerne slette min konto")]
            //[Range(typeof(bool),"true","true", ErrorMessage = "Vi kan ikke slette din konto, uden at du indvilliger i ovenstående")]
            public bool Aggreed { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password not correct.");
                    return Page();
                }
            }

            if (!Input.Aggreed)
            {
                ModelState.AddModelError(string.Empty ,"Du skal acceptere betingelserne for sletningen");
                return Page();
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                //throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
                ModelState.AddModelError(string.Empty, "Kontakt Dykkerklubben for hjælp til at slette din konto");

                return Page();
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            var admins = await _userManager.GetUsersInRoleAsync(Userroles.Administrator);
            foreach (var item in admins)
            {
                await _smsSender.SendSmsAsync(item.PhoneNumber, $"{user.UserName}' har ikke ønsket at være medlem længere og har slettet deres konto.");
                await _emailSender.SendEmailAsync(item.Email, "Bruger udmelding", $"{user.UserName}' har ikke ønsket at være medlem længere og har slettet deres konto<br/>" +
                    $"<br/>" +
                    $"Med venlig hilsen<br/>" +
                    $"<br/>" +
                    $"Klubkalenderen");
            }
            
            return Redirect("~/");
        }
    }
}