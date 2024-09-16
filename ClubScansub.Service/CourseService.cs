using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
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
            var data = context.Courses
                .Include(x=>x.CourseSessions)
                .Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Where(x=> x.StartDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public async Task<IList<Course>> GetStartedOrCompletedAsync()
        {
            var data = context.Courses
                .Include(x => x.CourseSessions)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.StartDateAndTime < DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public async Task<Course> GetAsync(string Id)
        {
            var id = int.Parse(Id);
            var course = await context.Courses
                .Include(x => x.CourseSessions)
                .Include(x=>x.Participants)
                .FirstAsync(x => x.Id == id);
            return course;
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

        // Session functions


        // Template functions
        public async Task<IList<CourseTemplate>> GetTemplatesAsync()
        {
            var data = context.CourseTemplates.Include(x => x.Sessions);
            return await data.ToListAsync();   
        }
    }
}
