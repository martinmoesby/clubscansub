using ClubScansub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Components.Image
{
    public class ImageViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public ImageViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int imageId, bool showThumbnail)
        {
            var image = await db.DivelocationImages.FirstOrDefaultAsync(x=>x.DivelocationId == imageId);
            if (image != null)
            {


                if (showThumbnail)
                    return View("Thumbnail", image);
                return View("Image", image);
            }

            return View("NoImage");
        }
    }
}
