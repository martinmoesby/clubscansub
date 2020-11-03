using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Members.Controllers
{
    [Area("Members")]
    [Authorize(Roles = Userroles.User)]
    public class CertificateController : BaseController
    {

        public CertificateController(ApplicationDbContext db)
            :base(db)
        {
        }

        [BindProperty]
        ApplicationUser CurrentUser { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            CurrentUser = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            var myCerts = await db.UserCertificates.Include(x=>x.Certificate)
                .Where(x => x.User == CurrentUser).ToListAsync();
            
            return View(myCerts);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cert = await db.UserCertificates.Include(x => x.Certificate).FirstOrDefaultAsync(x => x.Id == id);

            return View(cert);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserCertificat certificat)
        {
          
            var frontImage = HttpContext.Request.Form.Files["Front"];
            var backImage = HttpContext.Request.Form.Files["Back"];

            if (frontImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    frontImage.CopyTo(ms);
                    certificat.FrontSideImage = ms.ToArray();
                }
            }

            if (backImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    backImage.CopyTo(ms);
                    certificat.BackSideImage = ms.ToArray();
                }
            }


            db.Entry(certificat).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return Redirect("/Identity/Account/Manage/Certifications");

            //return RedirectToAction(nameof(Index));
        }

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var certificate = await db.UserCertificates.Include(x=>x.Certificate).Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id == id);

            if (certificate == null)
                return NotFound();

            return View(certificate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCertificate(UserCertificat userCertificat)
        {
            if (!ModelState.IsValid)
                return View(userCertificat);

            db.Attach(userCertificat).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            await db.SaveChangesAsync();

            return Redirect("/Identity/Account/Manage/Certifications");

//            return RedirectToAction(nameof(Index));

        }
    }
}