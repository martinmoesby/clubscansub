using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class CreateUserAccountTransactionViewModel
    {
        public IEnumerable<SelectListItem> Members { get; set; }

        public ApplicationUserAccountEntry Entry { get; set; }

        public string SelectedUserId { get; set; }

    }
}
