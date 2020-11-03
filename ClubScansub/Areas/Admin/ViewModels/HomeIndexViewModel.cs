using ClubScansub.Models;
using ClubScansub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class HomeIndexViewModel : BaseViewModel
    {
        public HomeIndexViewModel()
        {
            NewEvent = new CreateEventViewModel()
            {
                Event = new Models.Event()
            };
        }
        //public IEnumerable<SelectListItem> Locations { get; set; }
        //public IEnumerable<SelectListItem> Members { get; set; }

        public CreateEventViewModel NewEvent { get; set; }

        public CreateUserAccountTransactionViewModel NewTransaction { get; set; }

        public CreateCourseViewModel NewCourse { get; set; }

        public ApplicationUser AppUser { get; set; }
    }
}
