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
    /// <summary>
    /// Provides operations for managing courses, course sessions, instructors, and course templates within the
    /// application. Supports asynchronous creation, retrieval, updating, and deletion of course-related entities, as
    /// well as instructor assignment workflows.
    /// </summary>
    /// <remarks>CourseService implements ICourseService and extends ServiceBase to offer a comprehensive set
    /// of methods for handling course lifecycles, session scheduling, instructor approvals, and template management.
    /// Methods are designed for asynchronous usage and interact with the application's data context. This service is
    /// intended to be used by application logic that requires access to course and session management features,
    /// including instructor notifications via email and SMS. Thread safety is not guaranteed; instances should not be
    /// shared across concurrent operations.</remarks>
    public class CourseService : ServiceBase, ICourseService
    {
        private ISmsSender smsSender;

        public CourseService(IOptions<ServiceOptions> options, IEmailSender emailSender, ISmsSender smsSender) : base(options, emailSender)
        {

            this.smsSender = smsSender;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Course> AddAsync(Course Item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously adds the specified collection of courses to the database.
        /// </summary>
        /// <remarks>All courses in the provided list are added in a single database operation. Changes
        /// are saved immediately. This method does not update existing courses; all items are treated as new entities.
        /// Thread safety is not guaranteed; do not share the same context instance across threads.</remarks>
        /// <param name="Items">The list of <see cref="Course"/> objects to add. Cannot be null. Each course will be tracked and inserted as
        /// a new entity.</param>
        /// <returns>A list containing the added <see cref="Course"/> objects. The returned list is the same instance as the
        /// input parameter.</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Items"></param>
        /// <returns></returns>
        public Task<IList<Course>> AddAsync(params Course[] Items)
        {
            return AddAsync(Items.ToList());
        }

        /// <summary>
        /// Asynchronously deletes the specified course from the database context.
        /// </summary>
        /// <remarks>If the specified course is not found in the database, no changes are made. This
        /// method does not validate whether the entity exists prior to deletion.</remarks>
        /// <param name="Item">The course entity to be deleted. Must be attached to the context and represent an existing record.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteAsync(Course Item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.Attach(Item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await context.SaveChangesAsync();

        }
        /// <summary>
        /// Asynchronously retrieves all courses that have not yet ended, including their associated sessions and
        /// participants.
        /// </summary>
        /// <remarks>The returned courses include related CourseSessions and Participants, with each
        /// participant's associated ApplicationUser loaded. Only courses with an end date and time later than the
        /// current UTC time are included.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of courses with their
        /// related sessions and participants. The list will be empty if no active courses are found.</returns>
        public async Task<IList<Course>> GetAllAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Courses
                .Include(x=>x.CourseSessions)
                .Include(x=>x.Participants).ThenInclude(x=>x.ApplicationUser)
                .Where(x=> x.EndDateAndTime > DateTime.UtcNow);
            return await data.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Course>> GetStartedOrCompletedAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var data = context.Courses
                .Include(x => x.CourseSessions)
                .Include(x => x.Participants).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.StartDateAndTime < DateTime.UtcNow);
            return await data.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a list of instructors who have been approved for past course sessions.
        /// </summary>
        /// <remarks>Only instructors associated with course sessions that have already taken place, are
        /// approved, and have either registered participants or a positive fixed participant count are included in the
        /// result.</remarks>
        /// <returns>A list of <see cref="CourseSessionInstructor"/> objects representing instructors approved for course
        /// sessions that have already occurred. The list will be empty if no matching instructors are found.</returns>
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

        /// <summary>
        /// Asynchronously retrieves a list of completed course session instructor assignments for the specified
        /// instructor.
        /// </summary>
        /// <remarks>A course session is considered completed if its scheduled date and time are in the
        /// past, it has been approved by the instructor, and it has participants or a fixed participant count. The
        /// method filters results to include only those assignments associated with the provided instructor.</remarks>
        /// <param name="user">The instructor for whom to retrieve completed work. Must not be null. Only assignments associated with this
        /// user's identifier are returned.</param>
        /// <returns>A list of <see cref="CourseSessionInstructor"/> objects representing completed course sessions that have
        /// been approved by the instructor. The list will be empty if no completed work is found for the specified
        /// instructor.</returns>
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

        /// <summary>
        /// Asynchronously retrieves a course by its identifier, including its sessions and participants.
        /// </summary>
        /// <remarks>Throws an exception if the specified identifier does not correspond to an existing
        /// course or if the identifier is not a valid integer.</remarks>
        /// <param name="Id">The string representation of the course identifier. Must be convertible to an integer value corresponding to
        /// an existing course.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the course matching the
        /// specified identifier, including its sessions and participants.</returns>
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

        /// <summary>
        /// Asynchronously updates the specified course in the data store.
        /// </summary>
        /// <remarks>The update is performed immediately in the underlying data store. If the specified
        /// course does not exist, an exception may be thrown by the data context.</remarks>
        /// <param name="item">The course entity to update. Must not be null. The entity should have a valid identifier corresponding to an
        /// existing course.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated course entity.</returns>
        public async Task<Course> UpdateAsync(Course item)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.Update(item);
            await context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Asynchronously updates the specified collection of courses in the database.
        /// </summary>
        /// <remarks>All changes to the provided courses are persisted to the database upon successful
        /// completion of the operation. The returned list contains the same course instances as provided in <paramref
        /// name="Items"/>.</remarks>
        /// <param name="Items">The list of <see cref="Course"/> entities to update. Each item must represent an existing course in the
        /// database.</param>
        /// <returns>A list containing the updated <see cref="Course"/> entities.</returns>
        public async Task<IList<Course>> UpdateAsync(IList<Course> Items)
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.UpdateRange(Items);
            await context.SaveChangesAsync();
            return Items;
        }

        /// <summary>
        /// Updates the specified courses asynchronously.
        /// </summary>
        /// <param name="Items">An array of <see cref="Course"/> objects to update. Cannot be null. Each item represents a course to be
        /// updated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Course"/>
        /// objects reflecting the updated state of each course.</returns>
        public async Task<IList<Course>> UpdateAsync(params Course[] Items)
        {
            return await UpdateAsync(Items.ToList());
        }

        // Create but do not commit to database
        /// <summary>
        /// Creates a new course instance based on the specified course template and start date, without committing it
        /// to the database.
        /// </summary>
        /// <remarks>The returned Course object includes sessions and properties derived from the
        /// template, but is not persisted to the database. To save the course, use the appropriate data access method
        /// after creation.</remarks>
        /// <param name="courseTemplate">The template containing the course structure, sessions, and default settings. Cannot be null.</param>
        /// <param name="selectedStartDate">The date on which the course is intended to start. Must not be earlier than the current date.</param>
        /// <returns>A new Course object initialized from the template and start date, or null if the template is invalid or the
        /// start date is in the past.</returns>
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
        /// <summary>
        /// Retrieves all course sessions that occur within the specified month.
        /// </summary>
        /// <remarks>The returned sessions include related instructors, course details, and participants.
        /// The query is executed with no tracking for improved read performance.</remarks>
        /// <param name="date">A date representing the target month and year. Only the month and year components are used; the day
        /// component is ignored.</param>
        /// <returns>A list of <see cref="CourseSession"/> objects scheduled to start and end within the specified month. Returns
        /// an empty list if no sessions are found.</returns>
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

        /// <summary>
        /// Updates the specified course session entities in the database asynchronously.
        /// </summary>
        /// <remarks>All changes are persisted to the database upon successful completion of the
        /// operation. The method returns the same entities that were provided as input, reflecting their updated state.
        /// This operation is not atomic; if an error occurs during saving, some updates may not be applied.</remarks>
        /// <param name="items">An array of <see cref="CourseSession"/> objects to update. Each item must represent an existing session to
        /// be updated.</param>
        /// <returns>A list containing the updated <see cref="CourseSession"/> entities.</returns>
        public async Task<IList<CourseSession>> UpdateSessionAsync(params CourseSession[] items )
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            context.CourseSessions.UpdateRange(items);
            await context.SaveChangesAsync();
            return items.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="instructor"></param>
        /// <returns></returns>
        public async Task SignupSessionInstructor(CourseSession session, ApplicationUser instructor)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            await context.CourseSessionInstructors.AddAsync(new CourseSessionInstructor { CourseSessionId = session.Id, InstructorId = instructor.Id, InstructorApproved = false, InstructorRetracted = false });
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Approves or removes an instructor's assignment for a specified course session and notifies the instructor of
        /// the change.
        /// </summary>
        /// <remarks>If the instructor is approved or removed, an email and/or SMS notification is sent to
        /// the instructor if their contact information is available and confirmed. The method creates a new assignment
        /// if none exists and approval is requested.</remarks>
        /// <param name="session">The course session for which the instructor's assignment is being approved or removed.</param>
        /// <param name="instructor">The instructor whose assignment to the session is being updated. Cannot be null.</param>
        /// <param name="approved">Indicates whether the instructor is approved (<see langword="true"/>) or removed (<see langword="false"/>).
        /// Defaults to <see langword="true"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Retracts the specified instructor from the given course session, updating their approval status and
        /// notifying them via email and SMS if contact information is available.
        /// </summary>
        /// <remarks>If the instructor has a confirmed phone number or a valid email address, they will be
        /// notified of the retraction. The instructor's approval status is set to false, and their retraction status is
        /// toggled. No action is taken if the instructor is not associated with the session.</remarks>
        /// <param name="session">The course session from which the instructor will be retracted. Must not be null.</param>
        /// <param name="instructor">The instructor to retract from the session. Must not be null and should have valid contact information to
        /// receive notifications.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously retrieves all course templates, including their associated sessions, from the database.
        /// </summary>
        /// <remarks>The returned templates include their related session data. This method executes a
        /// database query and may incur network or I/O latency. The caller should await the result to ensure completion
        /// before accessing the returned data.</remarks>
        /// <returns>A list of <see cref="CourseTemplate"/> objects representing all course templates in the database. The list
        /// will be empty if no templates are found.</returns>
        public async Task<IList<CourseTemplate>> GetTemplatesAsync()
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            var data = context.CourseTemplates.Include(x => x.Sessions).ThenInclude(x=>x.Address);
            return await data.ToListAsync();   
        }

        /// <summary>
        /// Asynchronously adds a new course template and its associated sessions to the database.
        /// </summary>
        /// <remarks>The method saves the provided template and its sessions in a single transaction. The
        /// template's instructor certificate and session addresses are not tracked by the context after the operation.
        /// The returned template reflects the state after saving to the database.</remarks>
        /// <param name="template">The course template to add, including its sessions and optional instructor certificate. Cannot be null.</param>
        /// <returns>A <see cref="CourseTemplate"/> instance representing the added template, including any changes made during
        /// the save operation.</returns>
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

        /// <summary>
        /// Asynchronously deletes the specified course template from the database.
        /// </summary>
        /// <remarks>If the specified template is not tracked by the context, no changes will be made.
        /// This method does not throw if the template does not exist in the database.</remarks>
        /// <param name="template">The course template to delete. Must not be null and must exist in the current context.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteTemplateAsync(CourseTemplate template)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Remove(template);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the specified course template in the database asynchronously.
        /// </summary>
        /// <remarks>If the specified template does not exist in the database, the update may fail or
        /// result in an exception depending on the entity tracking configuration.</remarks>
        /// <param name="template">The course template entity to update. Must not be null and should represent an existing template in the
        /// database.</param>
        /// <returns>The updated <see cref="CourseTemplate"/> entity after changes have been saved to the database.</returns>
        public async Task<CourseTemplate> UpdateCourseTemplateAsync(CourseTemplate template)
        {
            using var context = new ApplicationDbContext(dbContextOptions);

            context.Update(template);
            await context.SaveChangesAsync();
            return template;
        }
    }
}
