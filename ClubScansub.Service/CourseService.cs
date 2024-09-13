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
            var data = context.Courses.Where(x=> x.StartDateAndTime > DateTime.UtcNow);
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


        public async Task<IList<EventUser>> GetUsersByCourse(int Id)
        {
            var users = await context.EventUsers.Include(x => x.ApplicationUser)
                .Where(x => x.EventId == Id).ToListAsync();
            return users;
        }

        public async Task<IList<EventUser>> EnrollUser(string userId, int CourseId)
        {
            await context.EventUsers.AddAsync(new EventUser() { ApplicationUserId = userId, EventId = CourseId });
            await context.SaveChangesAsync();
            return await GetUsersByCourse(CourseId);
        }

        public async Task RemoveUserFromCourse(string userId, int courseId)
        {
            var eventUser = await context.EventUsers.FindAsync(userId, courseId);
            if (eventUser != null)
            {
                context.EventUsers.Remove(eventUser);
                //TODO: Refund any payments for this course made to users account...
                await context.SaveChangesAsync();
            }
        }
    }
}
