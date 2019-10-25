using ClubScansub.Data;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Divepro.Controllers
{
    [Authorize(Roles = Userroles.Owner + ", " + Userroles.Administrator + ", " + Userroles.Divepro)]
    public class BaseDiveproController : BaseController
    {
        public BaseDiveproController(ApplicationDbContext db)
            : base(db)
        {

        }
    }
}
                    