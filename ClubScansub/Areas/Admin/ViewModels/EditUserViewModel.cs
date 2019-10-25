using ClubScansub.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class EditUserViewModel
    {
        public HashSet<string> Userroles { get; set; }

        public IEnumerable<IdentityRole> Roles { get; set; }

        public ApplicationUser User { get; set; }

    }
}
