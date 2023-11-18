using ClubScansub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    public abstract class ServiceBase
    {
        protected readonly DbContextOptions<ApplicationDbContext> dbContextOptions;
        //protected readonly IEmailSender emailSender;
        //protected readonly AppDbContext appDbContext;

        public ServiceBase(IOptions<ServiceOptions> options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(options.Value.ConnectionString);
            dbContextOptions = optionsBuilder.Options;
            //this.emailSender = emailSender;
        }
    }
}
