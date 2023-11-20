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
        IList<MemberDTO> GetByRoles(params string[] userroles);

    }
}
