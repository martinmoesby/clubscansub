using AutoMapper;
using ClubScansub.Data;
using ClubScansub.Models.DTO;
using ClubScansub.Service.Automapper;
using ClubScansub.Service.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ClubScansub.Service
{
    public class MemberService : ServiceBase, IMemberService
    {
        private readonly ApplicationDbContext context;
        private readonly Mapper mapper;

        public MemberService(IOptions<ServiceOptions> options, IEmailSender emailSender)
            : base( options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
            var config = new MemberMapperConfiguration().Configure();
            mapper = new Mapper(config);
            
        }

        public Task<MemberDTO> AddAsync(MemberDTO Item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> AddAsync(IList<MemberDTO> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> AddAsync(params MemberDTO[] Items)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(MemberDTO Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<MemberDTO>> GetAllAsync()
        {
            var members = await context.ApplicationUsers.ToListAsync();
            return mapper.Map<IList<MemberDTO>>(members);
        }

        public Task<MemberDTO> GetAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<MemberDTO> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> GetByRolesAsync(params string[] userroles)
        {
            
            throw new NotImplementedException();
        }


        public Task<MemberDTO> UpdateAsync(MemberDTO item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> UpdateAsync(IList<MemberDTO> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> UpdateAsync(params MemberDTO[] Items)
        {
            throw new NotImplementedException();
        }
    }
}
