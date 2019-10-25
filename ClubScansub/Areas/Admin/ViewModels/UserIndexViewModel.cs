using ClubScansub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class UserIndexViewModel
    {
        public IList<ApplicationUser> Users { get; set; }
        public Pager Pager { get; set; }

        public string SearchText { get; set; }

        public string MemberType { get; set; }

    }
}
