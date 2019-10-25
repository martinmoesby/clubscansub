using ClubScansub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.ViewModels
{
    public class CreateEventViewModel
    {
        public int DivelocationId { get; set; }
        public int CertificateId { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
        public IEnumerable<SelectListItem> Certificates { get; set; }
        public Event Event { get; set; }

    }
}
