using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.App_Data;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public class ImportAccountModel : PageModel
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext db;

        public ImportAccountModel(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.db = db;
            
        }

        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Kodeord")]
            public string Password { get; set; }


            [Required]
            [Display(Name = "Brugernummer")]
            public string Usernumber { get; set; }

            public bool AlreadyImported { get; set; }
        }

        public async void OnGet()
        {

            var user = await userManager.GetUserAsync(User);
            var appUser = await db.ApplicationUsers.FindAsync(user.Id);
            Input = new InputModel()
            {
                AlreadyImported = appUser.OldAccountImported
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            var appUser = await db.ApplicationUsers.FindAsync(user.Id);


            var oldMember = db.medlemsdata.Where(x => x.dsfnr == Input.Usernumber && x.password == Input.Password).FirstOrDefault();
            if (oldMember == null)
            {
                StatusMessage = "Ikke korrekt brugernavn eller kodeord, eller brugernummer ikke fundet";
                Input.AlreadyImported = false;
                return RedirectToPage();
            }

            if (oldMember.status)
            {
                await userManager.AddToRoleAsync(user, Userroles.Member);
            }

            var saldodata = db.saldooplysning.Where(x => x.dsfnr == Input.Usernumber);

            appUser.AccountNumber = Input.Usernumber.ToString();
            appUser.OldAccountImported = true;

            foreach (var item in saldodata)
            {
                var entry = new ApplicationUserAccountEntry
                {
                    Amount = item.pris,
                    AccountType = Utility.AccountTypeEnum.EventAccountType,
                    Description = $"{item.tekst} (Importeret fra gammel system)",
                    PostingDate = item.dato,
                    ApplicationUser = appUser
                };
                db.ApplicationUserAccountEntry.Add(entry);

            }

            //if (user == null)
            //    return NotFound();
            await db.SaveChangesAsync();
            Input.AlreadyImported = true;

            StatusMessage = "Din turkonto er nu blevet importeret.";
            return RedirectToPage("AccountBalance");// ("Edit", "User", new { id = userid });
        }
    }
}
