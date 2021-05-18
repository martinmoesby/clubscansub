using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Identity.Pages.Account.Manage
{
    public class CertificationsModel : PageModel
    {
        private readonly UserManager<IdentityUser> um;
        private readonly SignInManager<IdentityUser> sm;
        private readonly ApplicationDbContext db;
        public CertificationsModel(UserManager<IdentityUser> um, SignInManager<IdentityUser> sm, ApplicationDbContext db)
        {
            this.um = um;
            this.sm = sm;
            this.db = db;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }

        [BindProperty]
        public int SelectedCertificate { get; set; }

        [BindProperty]
        public IList<SelectListItem> Certificates { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var ownedCertificates = await db.UserCertificates.Include(x => x.User).Where(x => x.User.Id == User.GetIdentityId()).Select(x => x.Certificate).ToListAsync();


            Certificates = await db.Certificates
                .Where(x => !ownedCertificates.Contains(x))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ShortName }).ToListAsync();

            ApplicationUser = await db.ApplicationUsers.Include(x=>x.Certificates).ThenInclude(x=>x.Certificate).FirstOrDefaultAsync(x=>x.Id == User.GetIdentityId());

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var certificate = await db.Certificates.FindAsync(SelectedCertificate);
            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            var userCert = new UserCertificat
            {
                Certificate = certificate, 
                User = user
            };

            db.UserCertificates.Add(userCert);
            db.SaveChanges();

            var ownedCertificates = await db.UserCertificates.Include(x=>x.User).Where(x => x.User.Id == User.GetIdentityId()).Select(x=>x.Certificate).ToListAsync();

            ApplicationUser = await db.ApplicationUsers.Include(x => x.Certificates).ThenInclude(x=>x.Certificate).FirstOrDefaultAsync(x => x.Id == User.GetIdentityId());
            Certificates = db.Certificates.AsEnumerable().Where(x => !ownedCertificates.Any(c => x.Id == c.Id))
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ShortName }).ToList();

            return Page();
        }
    }
}