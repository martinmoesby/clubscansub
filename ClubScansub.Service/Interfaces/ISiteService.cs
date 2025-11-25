using ClubScansub.Models;
using ClubScansub.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface ISiteService : IGenericService<Divelocation>
    {
        Task<List<Certificate>> GetCertificatesForEvents();
        Task<List<Address>> GetMeetingLocations();

        Task RequestEventForDivesite(RequestEventDTO request);
    }
}
