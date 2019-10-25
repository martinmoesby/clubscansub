using System;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ApplicationDbContext db;

        public PersonalDataModel(
            UserManager<IdentityUser> userManager,
            ILogger<PersonalDataModel> logger,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _logger = logger;
            this.db = db;
        }

        public ApplicationUser ApplicationUser { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var data = await db.ApplicationUsers
                .Include(c => c.AccountTransactions)
                .Include(x => x.Events)
                    .ThenInclude(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            return Page();
        }
    }
}