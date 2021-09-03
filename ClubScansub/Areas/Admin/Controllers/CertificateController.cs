using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CertificateController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> um;
        public CertificateController(ApplicationDbContext db, UserManager<IdentityUser> um)
            :base(db)
        {
            this.um = um;

        }

        [BindProperty]
        public Certificate Certificate { get; set; }
        
        [BindProperty]
        public List<ApplicationUser> Divepros { get; set; }
        
        public async Task<IActionResult> Index()
        {
            var certificates = await db.Certificates.Include(x => x.Diveorgs).ThenInclude(x => x.Diveorg).ToListAsync();

            return View(certificates);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Certificate = new Certificate();
            var users = (await um.GetUsersInRoleAsync("Divepro"));
            Divepros = await db.ApplicationUsers.Where(x => users.Any(s => x.Id == s.Id)).ToListAsync();

            ViewData["Divepros"] = Divepros;

            return View(Certificate);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCert(int[] instructors)
        {
            if (!ModelState.IsValid)
                return View();

            foreach (var item in instructors)
            {
                Certificate.UserCertificate.Add(new UserCertificat()
                {
                    User = await db.ApplicationUsers.FindAsync(item),
                    IsVerified = true,
                    VerifiedBy = await db.ApplicationUsers.FindAsync(User.GetIdentityId())
                });
            }

            db.Certificates.Add(Certificate);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            int orgId = 0;
            if (!int.TryParse(id, out orgId))
            {
                return NotFound();
            }

            Certificate = await db.Certificates.Include(x=>x.UserCertificate).ThenInclude(x=>x.User).SingleOrDefaultAsync(x=>x.Id == orgId);

            var members = (await um.GetUsersInRoleAsync("Divepro")).Select(x=>x.Id);
            var pros = await db.ApplicationUsers.Where(x => members.Contains(x.Id)).ToListAsync();

            ViewData["Divepros"] = pros;

            return View(Certificate);

        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCert()
        {
            if (!ModelState.IsValid)
            {
                var members = (await um.GetUsersInRoleAsync("Divepro")).Select(x => x.Id);
                var pros = await db.ApplicationUsers.Where(x => members.Contains(x.Id)).ToListAsync();
                ViewData["Divepros"] = pros;

                return View(Certificate);
            }

            db.Entry(Certificate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> AddCertificateToUser(int certificateId, string instructorId)
        {
            var cert = await db.Certificates.FindAsync(certificateId);

            db.UserCertificates.Add(new UserCertificat()
            {
                Certificate = cert,
                User = await db.ApplicationUsers.FindAsync(instructorId),
                IsVerified = true,
                VerifiedBy = await db.ApplicationUsers.FindAsync(User.GetIdentityId())
            });

            await db.SaveChangesAsync();

            var users = await db.UserCertificates.Include(x => x.User).Where(x => x.Certificate == cert).ToListAsync();
           
            return Ok(Json(users.Select(X=> new { 
                InstructorId = X.User.Id,
                InstructorName = X.User.Name
            }))
            );

        }

        [HttpPost]
        public async Task<IActionResult>RemoveCertificateFromUser(int certificateId, string instructorId)
        {
            var cert = await db.Certificates.FindAsync(certificateId);

            var userCert = db.UserCertificates.Where(c => c.Certificate.Id == certificateId && c.User.Id == instructorId);
            db.UserCertificates.RemoveRange(userCert);

            await db.SaveChangesAsync();

            var users = await db.UserCertificates.Include(x => x.User).Where(x => x.Certificate == cert).ToListAsync();

            return Ok(Json(users.Select(X => new {
                InstructorId = X.User.Id,
                InstructorName = X.User.Name
            }))
            );

        }
    }
}