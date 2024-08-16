using ClubScansub.Data;
using ClubScansub.Service.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClubScansub.Service
{
    public abstract class ServiceBase
    {
        protected readonly DbContextOptions<ApplicationDbContext> dbContextOptions;
        protected readonly IEmailSender emailSender;
        protected ApplicationDbContext appDbContext;

        public ServiceBase(IOptions<ServiceOptions> options, IEmailSender emailSender)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(options.Value.ConnectionString);
            dbContextOptions = optionsBuilder.Options;

            this.emailSender = emailSender;
        }
    }
}
