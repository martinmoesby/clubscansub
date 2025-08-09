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
        IList<ApplicationUser> FindAll(string filter);
        Task<IList<Certificate>> GetAvailableCertificatesAsync(bool inclProCertificates);
        Task<IList<ApplicationUser>> GetByRolesAsync(params string[] userroles);

        Task<bool> IsUserInRole(ApplicationUser user, string role);

    }
}
