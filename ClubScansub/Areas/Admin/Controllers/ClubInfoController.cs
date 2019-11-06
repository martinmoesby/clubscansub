using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClubInfoController : BaseController
    {

        public ClubInfoController(ApplicationDbContext db)
            :base(db)
        {
            Setting = db.ClubSettings.SingleOrDefault();
        }

        [BindProperty]
        public ClubSettings Setting { get; set; }

        public IActionResult Index()
        {
            if (Setting == null)
                Setting = new ClubSettings();

            return View(Setting);

        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSetting()
        {

            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                using (var ms = new MemoryStream())
                {
                    files[0].CopyTo(ms);
                    Setting.Logo = ms.ToArray();
                }
            }
            var clubinfo = db.ClubSettings.SingleOrDefault();
            if (clubinfo == null)
                return NotFound();

            clubinfo.Name = Setting.Name;
            clubinfo.PrivacyText = Setting.PrivacyText;
            clubinfo.Logo = Setting.Logo;
            clubinfo.Tagline = Setting.Tagline;
            clubinfo.TermsText = Setting.TermsText;
            //db.Attach(Setting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}