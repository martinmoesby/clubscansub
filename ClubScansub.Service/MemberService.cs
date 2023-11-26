using AutoMapper;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service.Automapper;
using ClubScansub.Service.Interfaces;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
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
        private readonly UserManager<ApplicationUser> userManager;

        public MemberService(IOptions<ServiceOptions> options, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
            : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
            var config = new MemberMapperConfiguration().Configure();
            mapper = new Mapper(config);
            this.userManager = userManager;

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

        public async Task DeleteAsync(MemberDTO Item)
        {
            context.Attach(Item);
            context.Entry(Item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await context.SaveChangesAsync();

        }

        public async Task<IList<MemberDTO>> GetAllAsync()
        {
            var members = await context.ApplicationUsers.Include(x=>x.AccountTransactions).AsNoTracking().ToListAsync();
            var data = mapper.Map<IList<MemberDTO>>(members);

            return data;
        }

        public async Task<IList<MemberDTO>> GetAllByRolesAsync(string[] roles)
        {
            List<ApplicationUser> users = new();
            foreach (var item in roles)
            {
                var roleUsers = await userManager.GetUsersInRoleAsync(item);
                users.AddRange(roleUsers);
                users = users.Distinct().ToList(); ;
            }
            var members = await context.ApplicationUsers.Include(x => x.AccountTransactions).AsNoTracking().Where(x=> users.Contains(x)).ToListAsync();
            var data = mapper.Map<IList<MemberDTO>>(members);

            return data;
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

        public async Task<MemberDTO> UpdateAsync(MemberDTO item)
        {
            var member = mapper.Map<ApplicationUser>(item);

            context.Update(member);
            await context.SaveChangesAsync();
            context.Entry(member).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            return item;

        }

        public Task<IList<MemberDTO>> UpdateAsync(IList<MemberDTO> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MemberDTO>> UpdateAsync(params MemberDTO[] Items)
        {
            throw new NotImplementedException();
        }

        public async Task<MemberDTO> ToggleActiveStatus(MemberDTO member)
        {
            var user = context.ApplicationUsers.Find(member.Id);
            if (user == null)
                return member;
            user.LockoutEnd = user.LockoutEnd == DateTime.MaxValue ? DateTime.Now : DateTime.MaxValue;
            user.LockoutEnabled = true;
            await context.SaveChangesAsync();

            return mapper.Map<MemberDTO>(user);

        }

        public async Task<MemberDTO> AddTransactionAsync(MemberDTO member, AccountTransactionDTO transaction)
        {
            var user = await context.ApplicationUsers.FindAsync(member.Id);
            if (user == null)
                return member;

            transaction.ApplicationUser = user;
            context.ApplicationUserAccountEntry.Add(transaction);
            await context.SaveChangesAsync();

            var returnUser = context.ApplicationUsers.Include(x => x.AccountTransactions).FirstOrDefault(x => x.Id == user.Id);
            return mapper.Map<MemberDTO>(returnUser);


        }
    }
}
