using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class CreateCourseViewModel
    {
        public int CourseTemplateId { get; set; }
        public IEnumerable<SelectListItem> Coursetemplates { get; set; }
        public Course Course { get; set; }
    }
}
