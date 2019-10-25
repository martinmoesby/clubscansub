using ClubScansub.Data;
using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiveorganizationController : BaseAdminController
    {
        public DiveorganizationController(ApplicationDbContext db)
            :base(db)
        {
        }

        [BindProperty]
        public Diveorganization Diveorganization { get; set; }

        public class CertificateIndexViewModel
        {
            public IEnumerable<Diveorganization> Diveorganizations { get; set; }
            
        }

        public async Task<ActionResult> Index()
        {
            var Diveorgs = await db.Diveorganizations.Include(x=>x.Certificates).ToListAsync();
            return View(Diveorgs);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            int orgId = 0;
            if (!int.TryParse(id, out orgId))
            {
                return NotFound();
            }

            Diveorganization = await db.Diveorganizations.FindAsync(orgId);

            return View(Diveorganization);

        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrg()
        {
            if (!ModelState.IsValid)
                return View(Diveorganization);

            db.Entry(Diveorganization).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            Diveorganization = new Diveorganization();

            return View(Diveorganization);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrg()
        {
            if (!ModelState.IsValid)
                return View();

            db.Diveorganizations.Add(Diveorganization);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
