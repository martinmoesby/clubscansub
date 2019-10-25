using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class CreateCourseSessionTemplateViewModel
    {
        public int AddressId { get; set; }
        public CourseSessionTemplate SessionTemplate { get; set; }

        public List<SelectListItem> AddressItems { get; set; }
    }
}
