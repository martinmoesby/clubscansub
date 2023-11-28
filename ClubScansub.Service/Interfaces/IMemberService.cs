using ClubScansub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface IMemberService : IGenericService<ApplicationUser>
    {
        Task<IList<ApplicationUser>> GetByRolesAsync(params string[] userroles);

    }
}
