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
        Task<IList<CourseSession>> GetCourseSessionsByMonth(DateTime date);
        Task<IList<CourseTemplate>> GetTemplatesAsync();
    }
}
