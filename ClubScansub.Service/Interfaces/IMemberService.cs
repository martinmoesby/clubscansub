using ClubScansub.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface IMemberService : IGenericService<MemberDTO>
    {
        Task<IList<MemberDTO>> GetByRolesAsync(params string[] userroles);

    }
}
