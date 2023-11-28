using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClubScansub.Service
{
    public class MemberService : ServiceBase, IMemberService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public MemberService(IOptions<ServiceOptions> options, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
            : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
            this.userManager = userManager;

        }

        public Task<ApplicationUser> AddAsync(ApplicationUser Item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> AddAsync(IList<ApplicationUser> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> AddAsync(params ApplicationUser[] Items)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(ApplicationUser Item)
        {
            context.Attach(Item);
            context.Entry(Item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await context.SaveChangesAsync();

        }

        public async Task<IList<ApplicationUser>> GetAllAsync()
        {
            var members = await context.ApplicationUsers.Include(x=>x.AccountTransactions).ToListAsync();
            return members;
        }

        public async Task<IList<ApplicationUser>> GetAllByRolesAsync(string[] roles)
        {
            List<ApplicationUser> users = new();
            foreach (var item in roles)
            {
                var roleUsers = await userManager.GetUsersInRoleAsync(item);
                users.AddRange(roleUsers);
                users = users.Distinct().ToList(); ;
            }
            var members = await context.ApplicationUsers.Include(x => x.AccountTransactions).Where(x=> users.Contains(x)).ToListAsync();
            return members;
        }

        public Task<ApplicationUser> GetAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> GetByRolesAsync(params string[] userroles)
        {

            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser item)
        {
            var user = await context.ApplicationUsers.FindAsync(item.Id);
            if (user == null)
                return item;

            user.PhoneNumber = item.PhoneNumber;
            user.Email = item.Email;
            user.AccountNumber = item.AccountNumber;
            user.Firstname = item.Firstname;
            user.Lastname = item.Lastname;
            user.Streetaddress = item.Streetaddress;
            user.PostalCode = item.PostalCode;
            user.City = item.City;
            user.Country = item.Country;
            user.IsMultiUser = item.IsMultiUser;

            await context.SaveChangesAsync();

            return item;
        }

        public Task<IList<ApplicationUser>> UpdateAsync(IList<ApplicationUser> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> UpdateAsync(params ApplicationUser[] Items)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> ToggleActiveStatus(ApplicationUser member)
        {
            var user = context.ApplicationUsers.Find(member.Id);
            if (user == null)
                return member;
            user.LockoutEnd = user.LockoutEnd == DateTime.MaxValue ? DateTime.Now : DateTime.MaxValue;
            user.LockoutEnabled = true;
            await context.SaveChangesAsync();

            return user;

        }

        public async Task<ApplicationUser> AddTransactionAsync(ApplicationUser member, ApplicationUserAccountEntry transaction)
        {
            var user = await context.ApplicationUsers.Include(x => x.AccountTransactions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == member.Id);

            if (user == null)
                return member;

            user.AccountTransactions.Add(transaction);  
            await context.SaveChangesAsync();
            return user;

        }
    }
}
