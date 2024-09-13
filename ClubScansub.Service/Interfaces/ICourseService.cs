using ClubScansub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface ICourseService : IGenericService<Course>
    {
        Task<IList<EventUser>> EnrollUser(string userId, int CourseId);
        Task<IList<EventUser>> GetUsersByCourse(int Id);
        Task RemoveUserFromCourse(string id1, int id2);
    }
}
