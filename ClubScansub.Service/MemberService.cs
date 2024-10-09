using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Exceptions;
using ClubScansub.Service.Interfaces;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClubScansub.Service
{
    public class MemberService : ServiceBase, IMemberService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserStore<ApplicationUser> userStore;

        public MemberService(IOptions<ServiceOptions> options, IEmailSender emailSender, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
            : base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);
            this.userManager = userManager;
            this.userStore = userStore;
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
            var members = await context.ApplicationUsers.Include(x=>x.AccountTransactions).AsNoTracking().ToListAsync();

            foreach (var item in members)
            {
                item.Roles = (await userManager.GetRolesAsync(item)).ToArray();
            }

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
            var members = await context.ApplicationUsers.Include(x => x.AccountTransactions).Where(x=> users.Contains(x)).AsNoTracking().ToListAsync();
            return members;
        }

        public async Task<ApplicationUser> GetAsync(string Id)
        {
            var user = await context.ApplicationUsers.Include(x=>x.AccountTransactions).Include(x=>x.Certificates).ThenInclude(x=>x.Certificate).Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (user == null)
                throw new UserNotFoundException($"User with Id {Id} was not found.");

            return user;
        }

        public async Task<ApplicationUser> GetAsync(Guid Id)
        {
            throw new NotImplementedException();

        }

        public Task<IList<ApplicationUser>> GetByRolesAsync(params string[] userroles)
        {

            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser item)
        {
            if (item.Roles == null)
                throw new ArgumentNullException(nameof(item.Roles));

            var user = await userStore.FindByIdAsync(item.Id, new CancellationToken());
            if (user == null)
                throw new ArgumentException($"User '{item.Id}' could not be retrieved");

            try
            {
                var existingRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, existingRoles);
                await userManager.AddToRolesAsync(user, item.Roles);

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

                await userManager.UpdateAsync(user);


            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update user information.Se InnerExceptions for more information", ex);
            }

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
            //var user = context.ApplicationUsers.Find(member.Id);
            //if (user == null)
            //    return member;
            member.LockoutEnd = member.LockoutEnd == DateTime.MaxValue ? DateTime.Now : DateTime.MaxValue;
            member.LockoutEnabled = true;
            context.ApplicationUsers.Update(member);
            await context.SaveChangesAsync();

            return member;

        }

        public async Task<ApplicationUser> AddTransactionAsync(ApplicationUser member, ApplicationUserAccountEntry transaction)
        {
            var user = await context.ApplicationUsers.Include(x => x.AccountTransactions).FirstOrDefaultAsync(x => x.Id == member.Id);

            if (user == null)
                return member;

            user.AccountTransactions.Add(transaction);  

            await context.SaveChangesAsync();
            return user;

        }

        public IList<ApplicationUser> FindAll(string filter)
        {
            filter = filter.ToLower();
            var users = context.ApplicationUsers.AsQueryable();
            users = users.Where(x=>x.Firstname.ToLower().Contains(filter) || x.Lastname.ToLower().Contains(filter) || x.AccountNumber.Contains(filter));
            return users.ToList();
        }

        public async Task<IList<Certificate>> GetAvailableCertificatesAsync(bool inclProCertificates)
        {
            var data = context.Certificates.AsQueryable();
            if (inclProCertificates)
                return await data.ToListAsync();

            return await data.Where(x => x.IsDiveproCertificate == false).ToListAsync();        
        }
    }
}
