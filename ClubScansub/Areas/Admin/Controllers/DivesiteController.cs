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
    public class DivesiteController : BaseAdminController
    {
        public DivesiteController(ApplicationDbContext db)
            : base(db)
        {

            // Initilize Viewmodel
            DiveLocationViewModel = new CreateDivelocationViewModel()
            {
                Divelocation = new Divelocation(),
                SelectedAddressId = string.Empty,
                SelectedCertificateId = string.Empty,
                ExistingAddresses = db.Addresses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList(),
                Certificates = db.Certificates.Select(x=> new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList()
            };
            //DiveLocationViewModel.ExistingAddresses.Add(new SelectListItem { Selected = true, Value = "", Text = "" });
            DiveLocationViewModel.Divelocation.MeetingLocation = new Address();
            //End init

        }

        [BindProperty]
        public CreateDivelocationViewModel DiveLocationViewModel { get; set; }
        public async Task<IActionResult> Index()
        {
            var locations = await db.Divelocations.Include(x => x.MeetingLocation).ToListAsync();
            return View(locations);
        }

        #region Create Get Post

        [HttpGet]
        public IActionResult Create()
        {
            return View(DiveLocationViewModel);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
                return View(DiveLocationViewModel);

            //TODO: Logic to either get an existing address or create a new one
            if (!string.IsNullOrEmpty(DiveLocationViewModel.SelectedAddressId))
            {
                var existingAddress = await db.Addresses.FindAsync(int.Parse(DiveLocationViewModel.SelectedAddressId));

                if (existingAddress.Name != DiveLocationViewModel.Divelocation.MeetingLocation.Name)
                    existingAddress.Name = DiveLocationViewModel.Divelocation.MeetingLocation.Name;

                if (existingAddress.Streetname != DiveLocationViewModel.Divelocation.MeetingLocation.Streetname)
                    existingAddress.Streetname = DiveLocationViewModel.Divelocation.MeetingLocation.Streetname;

                if (existingAddress.Zipcode != DiveLocationViewModel.Divelocation.MeetingLocation.Zipcode)
                    existingAddress.Zipcode = DiveLocationViewModel.Divelocation.MeetingLocation.Zipcode;

                if (existingAddress.City != DiveLocationViewModel.Divelocation.MeetingLocation.City)
                    existingAddress.City = DiveLocationViewModel.Divelocation.MeetingLocation.City;

                if (existingAddress.Country != DiveLocationViewModel.Divelocation.MeetingLocation.Country)
                    existingAddress.Country = DiveLocationViewModel.Divelocation.MeetingLocation.Country;

                DiveLocationViewModel.Divelocation.MeetingLocation = existingAddress;
            }

            if (!string.IsNullOrEmpty(DiveLocationViewModel.SelectedAddressId))
            {
                var certificate = await db.Certificates.FindAsync(int.Parse(DiveLocationViewModel.SelectedAddressId));
                if (certificate != null)
                {
                    DiveLocationViewModel.Divelocation.Certificate = certificate;
                }
            }

            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                using (var ms = new MemoryStream())
                {
                    files[0].CopyTo(ms);
                    DiveLocationViewModel.Divelocation.Image = ms.ToArray();
                }
            }


            db.Divelocations.Add(DiveLocationViewModel.Divelocation);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        #endregion


        #region Delete Get Post 

        public async Task<IActionResult> Delete(int? id)
        {
            var divesite = await db.Divelocations.FindAsync(id);
            if (divesite == null)
                return NotFound();
            
            return View(divesite);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var divesite = await db.Divelocations.Include(x=> x.Events).FirstOrDefaultAsync(x=>x.Id == id);
            if (divesite == null)
                return NotFound();

            db.Divelocations.Remove(divesite);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion


        #region Edit Get Post
        public async Task<IActionResult> Edit (int id)
        {
            var divelocation = await db.Divelocations.Include(x=>x.MeetingLocation).Include(x=>x.Certificate).FirstOrDefaultAsync(x=>x.Id == id);
            if (divelocation == null)
                return NotFound();


            var model = new CreateDivelocationViewModel();
            model.Divelocation = divelocation;
            model.ExistingAddresses = await db.Addresses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            model.Certificates = db.Certificates.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            if (divelocation.MeetingLocation != null)
                model.SelectedAddressId = divelocation.MeetingLocation.Id.ToString();

            if (divelocation.Certificate != null)
                model.SelectedCertificateId = divelocation.Certificate.Id.ToString();

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost()
        {
            var model = DiveLocationViewModel.Divelocation;

            if (!string.IsNullOrEmpty(DiveLocationViewModel.SelectedCertificateId))
            {
                var certificate = await db.Certificates.FindAsync(int.Parse(DiveLocationViewModel.SelectedCertificateId));
                if (certificate != null)
                {
                    model.Certificate = certificate;
                }
            }

            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                using (var ms = new MemoryStream())
                {
                    files[0].CopyTo(ms);
                    model.Image = ms.ToArray();
                }
            }

            db.Attach(model);

            foreach (var item in db.ChangeTracker.Entries())
            {
                item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        #endregion

        public async Task<IActionResult> GetExistingAddress(int id)
        {
            var address = await db.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            return Json(address);
        }
    }
}