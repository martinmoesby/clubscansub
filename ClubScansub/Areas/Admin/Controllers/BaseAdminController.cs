using ClubScansub.Data;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Areas.Admin.Controllers
{

    [Authorize(Roles= Userroles.Administrator + ", " + Userroles.Owner)]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(ApplicationDbContext db)
            : base(db)
        {

        }
    }
}
