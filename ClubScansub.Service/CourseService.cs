using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using ClubScansub.Utility;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;

namespace ClubScansub.Service
{
    public class CourseService : ServiceBase, ICourseService
    {
        private ISmsSender smsSender;

        public CourseService(IOptions<ServiceOptions> options, IEmailSender emailSender, ISmsSender smsSender) : base(options, emailSender)
        {

            this.smsSender = smsSender;
        }

        public Task<Course> AddAsync(Course Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Course>> AddAsync(IList<Course> Items)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            foreach (var item in Items)
            {
                
                context.Attach(item).State = Microsoft.EntityFrameworkCore.EntityState.Added; 
                //foreach (var subitem in item.CourseSessions)
                //{
                //    context.Attach(subitem).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                //}
                
            }
            await context.SaveChangesAsync();
            return Items;

        }

        public Task<IList<Course>> AddAsync(params Course[] Items)
        {
            return AddAsync(Items.ToList());
        }

        public async Task DeleteAsync(Course Item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.Attach(Item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await context.SaveChangesAsync();

        }

        public async Task<IList<Course>> GetAllAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Courses
                .Include(x=>x.CourseSessions)
                .Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Where(x=> x.EndDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public async Task<IList<Course>> GetStartedOrCompletedAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Courses
                .Include(x => x.CourseSessions)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.StartDateAndTime < DateTime.UtcNow);
            return await data.ToListAsync();
        }

        public async Task<IList<CourseSessionInstructor>> GetCourseSessionInstructorAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.CourseSessionInstructors
                .Include(x => x.CourseSession)
                .ThenInclude(x => x.Course).ThenInclude(x => x.Participants)
                .Include(x => x.Instructor)
                //.OrderBy(x => x.CourseSession.Course.StartDateAndTime).ThenBy(x => x.CourseSession.DateTime).ThenBy(x => x.Instructor.UserName)
                .ToListAsync();

            var result = data.Where(x => x.CourseSession.DateTime < DateTime.UtcNow && x.InstructorApproved == true && (x.CourseSession.Course.Participants != null || x.CourseSession.Course.FixedParticipants > 0)).ToList();
            return result;
        }

        public async Task<IList<CourseSessionInstructor>> GetInstructorCompletedWorkAsync(ApplicationUser user)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var data = await context.CourseSessionInstructors
                .Include(x => x.CourseSession)
                .ThenInclude(x => x.Course).ThenInclude(x => x.Participants)
                .Include(x => x.Instructor)
                //.OrderBy(x => x.CourseSession.Course.StartDateAndTime).ThenBy(x => x.CourseSession.DateTime).ThenBy(x => x.Instructor.UserName)
                .ToListAsync();

            var result = data.Where(x => x.CourseSession.DateTime < DateTime.UtcNow 
                && x.InstructorApproved == true 
                && (x.CourseSession.Course.Participants != null 
                    || x.CourseSession.Course.FixedParticipants > 0)
                && x.Instructor.Id == user.Id
                ).ToList();
            return result;
        }

        public async Task<Course> GetAsync(string Id)
        {
            var id = int.Parse(Id);
            using var context = new ApplicationDbContext(dbContextOptions);

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


        public async Task<Course> UpdateAsync(Course item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.Update(item);
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<IList<Course>> UpdateAsync(IList<Course> Items)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.UpdateRange(Items);
            await context.SaveChangesAsync();
            return Items;
        }

        public async Task<IList<Course>> UpdateAsync(params Course[] Items)
        {
            return await UpdateAsync(Items.ToList());
        }

        // Create but do not commit to database

        public Course? CreateNewCourseFromTemplate(CourseTemplate courseTemplate, DateTime selectedStartDate)
        {
            if (selectedStartDate < DateTime.Now.Date || courseTemplate == null)
                return null;

            var firstSession = courseTemplate.Sessions.First();
            if (firstSession == null)
                return null;

            var course = new Course()
            {
                CourseName = courseTemplate.TemplateName,
                CourseTemplate = courseTemplate,
                CourseType = courseTemplate.CourseType,
                MinParticipants = courseTemplate.MinStudents,
                MaxParticipants = courseTemplate.MaxStudents,
                FixedParticipants = 0,
                Title = $"{courseTemplate.TemplateName} - {selectedStartDate.ToString("MMM")}",
                StartDateAndTime = selectedStartDate,
                Price = courseTemplate.DefaultPrice,
                PremiumPrice = courseTemplate.DefaultPrice,
                EventType = EventTypeEnum.NotAnEvent
            };

            var currentDate = course.StartDateAndTime;
            var currentWeekDay = course.StartDateAndTime.Date.DayOfWeek;
            // var previousDateTime = course.StartDateAndTime;
            // var prevoiusWeekDay = course.StartDateAndTime.Date.DayOfWeek;

            if (courseTemplate.Sessions.Count > 0)
            {
                course.StartDateAndTime += courseTemplate.Sessions.FirstOrDefault().DefaultStartTime;
                var courseSessions = new List<CourseSession>();


                foreach (var item in courseTemplate.Sessions.OrderBy(x => x.SessionNumber))
                {


                    if (item.UseDefaultWeekDay)
                    {
                        while (currentWeekDay != item.DefaultWeekday)
                        {
                            currentDate = currentDate.AddDays(1);
                            currentWeekDay = currentDate.DayOfWeek;
                        }
                    }

                    courseSessions.Add(new CourseSession()
                    {
                        CourseSessionTemplate = item,
                        DateTime = currentDate.Add(item.DefaultStartTime),
                        Sessiontype = item.SessionType,
                        Duration = item.DefaultDuration,
                        Address = item.Address,
                        SessionName = item.Name,
                        SessionDescription = item.Description,
                        Course = course
                    });
                    if (!item.UseDefaultWeekDay)
                    {
                        currentDate.AddDays(1);
                        currentWeekDay = currentDate.DayOfWeek;
                    }

                    // prevoiusWeekDay = currentWeekDay;
                    // previousDateTime = currentDate;

                }
                course.EndDateAndTime = courseSessions.Max(x => x.EndDateAndTime);
                course.CourseSessions = courseSessions;
            }

            return course;
        }

        // Session functions

        public async Task<IList<CourseSession>> GetCourseSessionsByMonth(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1).AddSeconds(-1);
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = await context.CourseSessions
                .Include(x => x.SessionInstructors)
                    .ThenInclude(x => x.Instructor)
                .Include(x => x.Course)
                    .ThenInclude(x=>x.Participants)
                        .ThenInclude(x=>x.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();

            return data.Where(x => x.StartDateAndTime >= start && x.EndDateAndTime <= end).ToList();

        }

        public async Task<IList<CourseSession>> UpdateSessionAsync(params CourseSession[] items )
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.CourseSessions.UpdateRange(items);
            await context.SaveChangesAsync();
            return items.ToList();
        }

        public async Task SignupSessionInstructor(CourseSession session, ApplicationUser instructor)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            await context.CourseSessionInstructors.AddAsync(new CourseSessionInstructor { CourseSessionId = session.Id, InstructorId = instructor.Id, InstructorApproved = false, InstructorRetracted = false });
            await context.SaveChangesAsync();
        }
        public async Task ApproveSessionInstructor(CourseSession session, ApplicationUser instructor, bool approved = true)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var sessionInstructor = context.CourseSessionInstructors.Where(x => x.InstructorId == instructor.Id && x.CourseSessionId == session.Id).FirstOrDefault();

            if (sessionInstructor != null)
            {
                sessionInstructor.InstructorApproved = approved;
                sessionInstructor.InstructorRetracted = false;
                context.CourseSessionInstructors.Update(sessionInstructor);
                try
                {

                    if (approved)
                    {
                        if (!string.IsNullOrEmpty(instructor.Email))
                        {
                            await emailSender.SendEmailAsync(instructor.Email, $"Instruction for {session.SessionName} approved", $"You have been assigned as instructor on the {session.SessionName} on {session.DateTime}");
                        }

                        if (instructor.PhoneNumberConfirmed && !string.IsNullOrEmpty(instructor.PhoneNumber))
                        {
                            await smsSender.SendSmsAsync(instructor.PhoneNumber, $"You have been assigned as instructor on {session.StartDateAndTime} - {session.SessionName}");
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(instructor.Email))
                        {
                            await emailSender.SendEmailAsync(instructor.Email, $"Instruction for {session.SessionName} removed", $"You have been removed as instructor on the {session.SessionName} on {session.DateTime}");
                        }

                        if (instructor.PhoneNumberConfirmed && !string.IsNullOrEmpty(instructor.PhoneNumber))
                        {
                            await smsSender.SendSmsAsync(instructor.PhoneNumber, $"You have been removed as instructor on {session.StartDateAndTime} - {session.SessionName}");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to notify Instructor due to: {ex.Message}");
                }
                finally
                {
                    await context.SaveChangesAsync();
                }
            } 
            else
            {
                if (sessionInstructor == null || approved == true)
                {
                    sessionInstructor = new CourseSessionInstructor { CourseSessionId = session.Id, InstructorId = instructor.Id, InstructorApproved = true, InstructorRetracted = false };
                    await context.CourseSessionInstructors.AddAsync(sessionInstructor);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task RetractSessionInstructor(CourseSession session, ApplicationUser instructor)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var sessionInstructor = context.CourseSessionInstructors.Where(x => x.InstructorId == instructor.Id && x.CourseSessionId == session.Id).FirstOrDefault();  //new CourseSessionInstructor { CourseSessionId = session.Id, InstructorId = instructor.Id, InstructorApproved = true, InstructorRetracted = false };
            if (sessionInstructor != null)
            {
                sessionInstructor.InstructorApproved = false;
                sessionInstructor.InstructorRetracted = !sessionInstructor.InstructorRetracted;
                context.CourseSessionInstructors.Update(sessionInstructor);

                if (!string.IsNullOrEmpty(instructor.Email))
                {
                    await emailSender.SendEmailAsync(instructor.Email, $"Instruction for {session.SessionName} retracted", $"You have been removed as instructor on the {session.SessionName} on {session.DateTime}");
                }

                if (instructor.PhoneNumberConfirmed && !string.IsNullOrEmpty(instructor.PhoneNumber))
                {
                    await smsSender.SendSmsAsync(instructor.PhoneNumber, $"You have been retracted as instructor on {session.StartDateAndTime} - {session.SessionName}");
                }

                await context.SaveChangesAsync();
            }

            //context.Update(session);
            //await context.SaveChangesAsync();
        }

        // Template functions
        public async Task<IList<CourseTemplate>> GetTemplatesAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var data = context.CourseTemplates.Include(x => x.Sessions);
            return await data.ToListAsync();   
        }

        public async Task<CourseTemplate> AddTemplateAsync(CourseTemplate template)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Entry(template).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            if (template.InstructorCertificate != null)
                context.Entry(template.InstructorCertificate).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            foreach (var item in template.Sessions)
            {
                context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                if (item.Address != null)
                    context.Entry(item.Address).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            //context.Add<CourseTemplate>(template);
            await context.SaveChangesAsync();
            return template;
        }

        public async Task DeleteTemplateAsync(CourseTemplate template)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Remove(template);
            await context.SaveChangesAsync();
        }

        public async Task<CourseTemplate> UpdateCourseTemplateAsync(CourseTemplate template)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Update(template);
            await context.SaveChangesAsync();
            return template;
        }
    }
}
