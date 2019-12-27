using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public class AccountBalanceModel : PageModel
    {
        private readonly UserManager<IdentityUser> um;
        private readonly SignInManager<IdentityUser> sm;
        private readonly ApplicationDbContext db;

        public AccountBalanceModel(UserManager<IdentityUser> um, SignInManager<IdentityUser> sm, ApplicationDbContext db)
        {
            this.um = um;
            this.sm = sm;
            this.db = db;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        [BindProperty]
        public IEnumerable<ApplicationUserAccountEntry> AccountTransactions { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ApplicationUser = await db.ApplicationUsers.FindAsync(um.GetUserId(User));
            AccountTransactions = await db.ApplicationUserAccountEntry.Where(c => c.ApplicationUser.Id == um.GetUserId(User) && c.AccountType == AccountTypeEnum.EventAccountType).OrderByDescending(x => x.PostingDate).ThenBy(x => x.Id).ToListAsync();
            return Page();
        }
    }
}