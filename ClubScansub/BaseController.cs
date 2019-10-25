using ClubScansub.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext db;
        public BaseController(ApplicationDbContext db)
        {
            this.db = db;
        }
    }
}
