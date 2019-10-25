using ClubScansub.Models;
using ClubScansub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class PlannerViewModel : BaseViewModel
    {

        public DateTime ActiveDate { get; set; }
        public IEnumerable<CourseTemplate> Courses { get; set; }

        public IEnumerable<Divelocation> Divesites { get; set; }
    }
}
