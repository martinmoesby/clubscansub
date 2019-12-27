using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.App_Data;
using Microsoft.AspNetCore.Mvc;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OldMemberDBController : Controller
    {
        public IActionResult Index()
        {
            //var repo = new OdbcRepository();

            return View();
        }
    }
}