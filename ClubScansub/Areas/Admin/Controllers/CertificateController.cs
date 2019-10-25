using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CertificateController : BaseAdminController
    {

        public CertificateController(ApplicationDbContext db)
            :base(db)
        {

        }

        [BindProperty]
        public Certificate Certificate { get; set; }
        public async Task<IActionResult> Index()
        {
            var certificates = await db.Certificates.Include(x => x.Diveorgs).ThenInclude(x => x.Diveorg).ToListAsync();

            return View(certificates);

        }

        [HttpGet]
        public IActionResult Create()
        {
            Certificate = new Certificate();

            return View(Certificate);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCert()
        {
            if (!ModelState.IsValid)
                return View();

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

            Certificate = await db.Certificates.FindAsync(orgId);

            return View(Certificate);

        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCert()
        {
            if (!ModelState.IsValid)
                return View(Certificate);

            db.Entry(Certificate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}