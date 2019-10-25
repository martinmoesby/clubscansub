using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class CreateDivelocationViewModel
    {
        public Divelocation Divelocation { get; set; }
        public IList<SelectListItem> ExistingAddresses { get; set; }

        public string SelectedAddressId { get; set; }
    }
}
