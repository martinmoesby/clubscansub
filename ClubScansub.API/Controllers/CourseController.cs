using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClubScansub.API.Controllers
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CourseController : BaseController
    {

        private UserManager<IdentityUser> userManager;

        public CourseController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(db)
        {
            this.userManager = userManager;

        }

        [Route("")]
        [HttpGet]
        public async Task<ActionResult> GetOpenCourses()
        {

            if (!(User.IsInRole(Userroles.Divepro) || User.HasClaim("IsDivePro", "True")))
                return Unauthorized();

            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            if (user == null)
                return Unauthorized();
            var myProCerts = db.UserCertificates
                .Include(x => x.User)
                .Include(x => x.Certificate)
                .Where(x => x.User.Id == User.GetIdentityId() && x.Certificate.IsDiveproCertificate == true);

            var courses = await db.Courses
                .Include(x=>x.CourseTemplate).ThenInclude(x=>x.InstructorCertificate)
                .Where(x => !x.IsCancelled && myProCerts.Any(y => y.Certificate.Id == x.CourseTemplate.InstructorCertificate.Id) && x.EndDateAndTime > DateTime.Now && x.StartDateAndTime < DateTime.Now.AddMonths(12))
                .Select(x=> new
                {
                    CourseId = x.Id,
                    x.CourseName,
                    x.StartDateAndTime,
                    x.EndDateAndTime,
                    x.InstructorsCovered,
                    //Students = x.Signups.Count,
                    CourseType= x.CourseType,
                    MinParticipantsRequired = x.MinParticipants,
                    CurrentParticipants = x.Participants.Count,
                    Students = x.Participants.Select(x=> new
                    {
                        x.ApplicationUser.Name,
                        x.ApplicationUser.PhoneNumber,
                        Id = x.ApplicationUserId
                    }),
                    Academic = x.CourseSessions.Count(y=>y.Sessiontype == CourseSessionTypeEnum.AcademicSession),
                    Pool = x.CourseSessions.Count(y => y.Sessiontype == CourseSessionTypeEnum.PoolSession),
                    Practice = x.CourseSessions.Count(y => y.Sessiontype == CourseSessionTypeEnum.OpenWaterSession)
                    //CourseSessions = x.CourseSessions.Select(cs => new
                    //{
                    //    SessionName = string.IsNullOrEmpty(cs.SessionName) ? cs.CourseSessionTemplate.Name : cs.SessionName,
                    //    cs.DateTime,
                    //    SessionDescription = string.IsNullOrEmpty(cs.SessionDescription) ? cs.CourseSessionTemplate.Description : cs.SessionDescription,
                    //    cs.Sessiontype,
                    //    SessionId = cs.Id,
                    //    LocationName = cs.Divelocation == null ? cs.Address == null ? "" : cs.Address.Name : cs.Divelocation.Name,
                    //    CurrentUserIsSignedUp = cs.SessionInstructors.Any(si => si.InstructorId == user.Id),
                    //    CurrentUserIsApproved = cs.SessionInstructors.Where(sia => sia.InstructorApproved).Any(z => z.InstructorId == user.Id),
                    //    Instructors = cs.SessionInstructors.Select(i => new
                    //    {
                    //        i.Instructor.Name,
                    //        IsApproved = i.InstructorApproved
                    //    })
                    //})

                }).OrderBy(x=>x.StartDateAndTime).ToListAsync();

            return Json(courses);

        }

        [Route("signup")]
        [HttpPost]
        public async Task<ActionResult> InstructorSignup([FromBody] SignupSessionId model) //[FromBody] SignupSessionId model
        {
            //SignupSessionId model = new SignupSessionId();

            if (!(User.IsInRole(Userroles.Divepro) || User.HasClaim("IsDivePro", "True")))
                return Unauthorized();

            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            if (user == null)
                return Unauthorized();


            var session = await db.CourseSessions
                .Include(x=>x.SessionInstructors)
                .Include(x=>x.Course)
                .FirstOrDefaultAsync(x => x.Id == model.SessionId);

            if (session.SessionInstructors.Any(x => x.InstructorId == user.Id))
                return Unauthorized(new { data="Instructor already signed up"});

            session.SessionInstructors.Add(new CourseSessionInstructor() { InstructorId = User.GetIdentityId() });

            //var sessioninstructor = new CourseSessionInstructor()
            //{
            //    CourseSessionId = sessionid,
            //    InstructorId = User.GetIdentityId()
            //};

            //db.CourseSessionInstructors.Add(sessioninstructor);
            await db.SaveChangesAsync();

            return await GetCourseSessions(session.Course.Id);
        }

        public class SignupSessionId
        {
            public int SessionId { get; set; }
        }


        [Authorize]
        [Route("{id}/sessions")]
        [HttpGet]
        public async Task<ActionResult> GetCourseSessions(int id)
        {

            if (!(User.IsInRole(Userroles.Divepro) || User.HasClaim("IsDivePro", "True")))
                return Unauthorized();

            var user = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            if (user == null)
                return Unauthorized();

            //CourseSessions = x.CourseSessions.Select(cs => new
            //{
            //SessionName = string.IsNullOrEmpty(cs.SessionName) ? cs.CourseSessionTemplate.Name : cs.SessionName,
            //    cs.DateTime,
            //    SessionDescription = string.IsNullOrEmpty(cs.SessionDescription) ? cs.CourseSessionTemplate.Description : cs.SessionDescription,
            //    cs.Sessiontype,
            //    SessionId = cs.Id,
            //    LocationName = cs.Divelocation == null ? cs.Address == null ? "" : cs.Address.Name : cs.Divelocation.Name,
            //    CurrentUserIsSignedUp = cs.SessionInstructors.Any(si => si.InstructorId == user.Id),
            //    CurrentUserIsApproved = cs.SessionInstructors.Where(sia => sia.InstructorApproved).Any(z => z.InstructorId == user.Id),
            //    Instructors = cs.SessionInstructors.Select(i => new
            //    {
            //        i.Instructor.Name,
            //        IsApproved = i.InstructorApproved
            //    })
            //})

            var sessions = await db.CourseSessions.Where(x => x.Course.Id == id && x.DateTime > DateTime.Now)
                .Select(cs => new
                {
                    SessionName = string.IsNullOrEmpty(cs.SessionName) ? cs.CourseSessionTemplate.Name : cs.SessionName,
                    cs.DateTime,
                    SessionDescription = string.IsNullOrEmpty(cs.SessionDescription) ? cs.CourseSessionTemplate.Description : cs.SessionDescription,
                    cs.Sessiontype,
                    SessionId = cs.Id,
                    LocationName = cs.Divelocation == null ? cs.Address == null ? "" : cs.Address.Name : cs.Divelocation.Name,
                    CurrentUserIsSignedUp = cs.SessionInstructors.Any(si => si.InstructorId == user.Id),
                    CurrentUserIsApproved = cs.SessionInstructors.Where(sia => sia.InstructorApproved).Any(z => z.InstructorId == user.Id),
                    Instructors = cs.SessionInstructors.Select(i => new
                    {
                        i.Instructor.Name,
                        IsApproved = i.InstructorApproved
                    })
                }).OrderBy(x=>x.DateTime).ToListAsync();

            return Json(sessions);

        }

    }
}
