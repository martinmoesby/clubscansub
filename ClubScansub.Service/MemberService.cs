using ClubScansub.Data;
using ClubScansub.Models.DTO;
using ClubScansub.Service.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    internal class MemberService :ServiceBase, IMemberService
    {
        public MemberService(IOptions<ServiceOptions> options, IEmailSender emailSender)
            : base( options, emailSender)
        {

        }
        public MemberDTO Add(MemberDTO Item)
        {
            throw new NotImplementedException();
        }

        public IList<MemberDTO> Add(IList<MemberDTO> Items)
        {
            throw new NotImplementedException();
        }

        public IList<MemberDTO> Add(params MemberDTO[] Items)
        {
            throw new NotImplementedException();
        }

        public void Delete(MemberDTO Item)
        {
            throw new NotImplementedException();
        }

        public MemberDTO Get(string Id)
        {
            throw new NotImplementedException();
        }

        public MemberDTO Get(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<MemberDTO>> GetAll()
        {
            using var context = new ApplicationDbContext(dbContextOptions);
            var members = context.ApplicationUsers;


        }

        public IList<MemberDTO> GetByRoles(params string[] userroles)
        {
            throw new NotImplementedException();
        }

        public MemberDTO Update(MemberDTO item)
        {
            throw new NotImplementedException();
        }

        public IList<MemberDTO> Update(IList<MemberDTO> Items)
        {
            throw new NotImplementedException();
        }

        public IList<MemberDTO> Update(params MemberDTO[] Items)
        {
            throw new NotImplementedException();
        }
    }
}
