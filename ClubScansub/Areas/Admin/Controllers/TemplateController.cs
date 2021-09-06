using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TemplateController : BaseAdminController
    {
        public TemplateController(ApplicationDbContext db)
            :base(db)
        {
            Certificates = db.Certificates.Where(x => x.IsDiveproCertificate).ToList();
        }

        public async Task<IActionResult> Index()
        {

            var templates = await db.CourseTemplates
                .Include(x => x.Sessions)
                .Include(x=>x.InstructorCertificate)
                .ToListAsync();

            return View(templates);

        }

        [BindProperty]
        public CourseTemplate CourseTemplate { get; set; }

        [BindProperty]
        public List<Certificate> Certificates { get; set; }

        [BindProperty]
        public CreateCourseSessionTemplateViewModel SessionTemplateVM { get; set; }



        #region Create Get Post

        [HttpGet]
        public IActionResult Create()
        {
            Certificates = db.Certificates.Where(x => x.IsDiveproCertificate).ToList();

            ViewData["Certificates"] = Certificates.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.ShortName }).ToList();

            return View(new CourseTemplate() { InstructorCertificate = new Certificate() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseTemplate model, int InstructorCerfificateId)
        {


            if (!ModelState.IsValid)
                return View();

            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                using (var ms = new MemoryStream())
                {
                    files[0].CopyTo(ms);
                    model.Image = ms.ToArray();
                }
            }
            var certificate = await db.Certificates.FindAsync(InstructorCerfificateId);
            model.InstructorCertificate = certificate;

            db.CourseTemplates.Add(model);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = model.Id });

        }


        #endregion

        #region Edit Get Post

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Certificates = db.Certificates.Where(x => x.IsDiveproCertificate).ToList();

            var template = await db.CourseTemplates
                .Include(x=>x.Sessions)
                .ThenInclude(x=>x.Address)
                .Include(x=>x.InstructorCertificate)
                .FirstOrDefaultAsync(x=>x.Id ==id);
            template.Sessions.OrderBy(x => x.SessionNumber);

            ViewData["Certificates"] = Certificates.Select(x=> new SelectListItem() { Value = x.Id.ToString(), Text = x.ShortName }).ToList();

            return View(template);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTemplate(CourseTemplate template)
        {
            var certificate = await db.Certificates.FindAsync(template.InstructorCertificate.Id);
            template.InstructorCertificate = certificate;

            db.Entry(template).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                using (var ms = new MemoryStream())
                {
                    files[0].CopyTo(ms);
                    template.Image = ms.ToArray();
                }
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        #endregion

        #region Delete Get Post
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            CourseTemplate = await db.CourseTemplates.Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == id);
            return View(CourseTemplate);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTemplate()
        {
            var template = await db.CourseTemplates.Include(x=>x.Sessions).FirstOrDefaultAsync(x=>x.Id == CourseTemplate.Id);
            template.Sessions.Clear();
            db.CourseTemplates.Remove(template);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region CreateEditDelete Session Get Post
        [HttpGet]
        public async Task<PartialViewResult> CreateSession(int id)
        {
            SessionTemplateVM = new CreateCourseSessionTemplateViewModel()
            {
                AddressItems = await db.Addresses.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync(),
                SessionTemplate = new CourseSessionTemplate()
                {
                    CourseTemplate = await db.CourseTemplates.FindAsync(id)
                }
            };

            return PartialView("_CreateTemplateSessionPartial", SessionTemplateVM);
        }

        [HttpPost, ActionName("CreateSession")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEditSessionPost(int CourseTemplateId, int CourseSessionTemplateId)
        {
            var address = await db.Addresses.FindAsync(SessionTemplateVM.AddressId);
            if (address != null)
            {
                SessionTemplateVM.SessionTemplate.Address = address;
            }

            var coursetemplate = await db.CourseTemplates.Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == CourseTemplateId);

            if (CourseSessionTemplateId != 0)
            {
                var sessiontemplate = await db.CourseSessionTemplates.FindAsync(CourseSessionTemplateId);

                sessiontemplate.SessionNumber = SessionTemplateVM.SessionTemplate.SessionNumber;
                sessiontemplate.SessionType = SessionTemplateVM.SessionTemplate.SessionType;
                sessiontemplate.Name = SessionTemplateVM.SessionTemplate.Name;
                sessiontemplate.DefaultDuration = SessionTemplateVM.SessionTemplate.DefaultDuration;
                sessiontemplate.Description = SessionTemplateVM.SessionTemplate.Description;
                sessiontemplate.DefaultStartTime = SessionTemplateVM.SessionTemplate.DefaultStartTime;
                sessiontemplate.DefaultWeekday = SessionTemplateVM.SessionTemplate.DefaultWeekday;
                sessiontemplate.UseDefaultWeekDay = SessionTemplateVM.SessionTemplate.UseDefaultWeekDay;
                sessiontemplate.Address = address;
            }
            else
            {
                coursetemplate.Sessions.Add(SessionTemplateVM.SessionTemplate);
            }

            coursetemplate.AcademicSessions = coursetemplate.Sessions.Count(x => x.SessionType == Utility.CourseSessionTypeEnum.AcademicSession);
            coursetemplate.PoolSessions = coursetemplate.Sessions.Count(x => x.SessionType == Utility.CourseSessionTypeEnum.PoolSession); 
            coursetemplate.OpenWaterSessions = coursetemplate.Sessions.Count(x => x.SessionType == Utility.CourseSessionTypeEnum.OpenWaterSession);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = CourseTemplateId });
        }

        [HttpGet]
        public async Task<PartialViewResult> EditSession(int id)
        {
            //SessionTemplateVM = new CreateCourseSessionTemplateViewModel()
            //{
            //    AddressItems = await db.Addresses.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync(),
            //    SessionTemplate = new CourseSessionTemplate()
            //    {
            //        CourseTemplate = await db.CourseTemplates.FindAsync(id)
            //    }
            //};
            SessionTemplateVM = new CreateCourseSessionTemplateViewModel()
            {
                SessionTemplate = await db.CourseSessionTemplates.Include(x => x.CourseTemplate).Include(x=>x.Address).FirstOrDefaultAsync(x => x.Id == id),
                AddressItems = await db.Addresses.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync()
            };
            if (SessionTemplateVM.SessionTemplate.Address != null)
            {
                SessionTemplateVM.AddressId = SessionTemplateVM.SessionTemplate.Address.Id;
            }

            return PartialView("_CreateTemplateSessionPartial", SessionTemplateVM);
        }

        public async Task<IActionResult>DeleteSession(int id)
        {
            //var coursetemplate = await db.CourseTemplates.Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == SessionTemplate.CourseTemplate.Id);
            var SessionTemplate = await db.CourseSessionTemplates.Include(c=>c.CourseTemplate).FirstOrDefaultAsync(x=>x.Id == id);

            db.Entry(SessionTemplate).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = SessionTemplate.CourseTemplate.Id });
        }
        #endregion
    }
}