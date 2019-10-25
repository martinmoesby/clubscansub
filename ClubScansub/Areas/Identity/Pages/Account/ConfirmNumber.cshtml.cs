using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClubScansub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClubScansub.Areas.Identity.Pages.Account
{
    public class ConfirmNumberModel : PageModel
    {
        private ApplicationDbContext db;
        private readonly UserManager<IdentityUser> um;

        public ConfirmNumberModel(UserManager<IdentityUser> um, ApplicationDbContext db)
        {
            this.um = um;
            this.db = db;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        [Display(Name ="Angiv den 6 cifrede kode")]
        public string VerificationCode { get; set; }

        [BindProperty]
        public string Phonenumber { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await um.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{um.GetUserId(User)}'.");
            }

            var phonenumber = await um.GetPhoneNumberAsync(user);
            if (phonenumber == null || string.IsNullOrEmpty(phonenumber))
            {
                return NotFound("Kunne ikke finde dit telefon nummer - tjek din profil");
                
            }

            Phonenumber = phonenumber;

            return Page();
        }

        public async Task<IActionResult> OnPostPhoneVerification()
        {
            var user = await db.ApplicationUsers.FindAsync(um.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{um.GetUserId(User)}'.");
            }

            var phonenumber = Regex.Replace(await um.GetPhoneNumberAsync(user),@"\s+","");

            var verificationResult = await um.VerifyChangePhoneNumberTokenAsync(user, VerificationCode, phonenumber);


            StatusMessage = "Dit telefonnummer er blevet  verificeret - du er nu klar til at modtage SMS'er fra systemet";

            if (!verificationResult)
            {
                StatusMessage = "Fejl: Kunne ikke verificere dit telefon nummer. Prøv at sende en ny kode";
                return Page();
            }
            else
            {
                user.PhoneNumberConfirmed = true;
                await db.SaveChangesAsync();

                return LocalRedirect("/Identity/Account/Manage/Index");
            }
            
        }
    }
}