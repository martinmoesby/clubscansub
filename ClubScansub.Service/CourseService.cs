using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClubScansub.Service
{
    public class CourseService : ServiceBase, ICourseService
    {
        private readonly ApplicationDbContext context;

        public CourseService(IOptions<ServiceOptions> options, IEmailSender emailSender) : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
        }

        public Task<Course> AddAsync(Course Item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Course>> AddAsync(IList<Course> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Course>> AddAsync(params Course[] Items)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Course Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Course>> GetAllAsync()
        {
            var data = context.Courses.Where(x=> x.StartDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public Task<Course> GetAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Course> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<Course> UpdateAsync(Course item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Course>> UpdateAsync(IList<Course> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Course>> UpdateAsync(params Course[] Items)
        {
            throw new NotImplementedException();
        }
    }
}
