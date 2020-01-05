using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubScansub.Areas.Admin.ViewModels;
using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ClubScansub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseAdminController
    {
        UserManager<IdentityUser> um;
        RoleManager<IdentityRole> rm;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
            : base(db)
        {
            this.um = um;
            this.rm = rm;
        }

        [BindProperty]
        public UserIndexViewModel IndexPageVM { get; set; }

        public class UserAccountTransactions
        {
            public string userId { get; set; }
            public AccountTypeEnum AccountType { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index(int page = 1, string searchText = "", string membertype = Userroles.User)
        {
            IndexPageVM = new UserIndexViewModel
            {
                SearchText = searchText ?? "",
                MemberType = membertype ?? Userroles.User
            };
            var usersinrole = await um.GetUsersInRoleAsync(membertype);

            var userSearchResult = await db.ApplicationUsers.Include(x => x.AccountTransactions).Include(c=>c.Certificates)
                .Where(x=>x.Name.ToLower().Contains(IndexPageVM.SearchText.ToLower()) && usersinrole.Any(u => x.Id == u.Id)               
                ).OrderBy(x=>x.AccountNumber).ToListAsync();

            IndexPageVM.Pager = new Pager
            {
                PageSize = 100,
                urlParam = $"/Admin/User/?page=:&searchText={searchText}&membertype={membertype}"
            };

            IndexPageVM.Pager.TotalItems = userSearchResult.Count();
            if (page > IndexPageVM.Pager.totalPages) page = 1;
            IndexPageVM.Pager.CurrentPage = page;

            var users = userSearchResult.Skip((page - 1) * IndexPageVM.Pager.PageSize).Take(IndexPageVM.Pager.PageSize).ToList();

            IndexPageVM.Users = users;

            return View(IndexPageVM);
        }

        public IActionResult Create()
        {
            return RedirectToPagePermanent("/Identity/Account/Register",new { returnUrl = "/Admin/User" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await db.ApplicationUsers
                .Include(x=>x.Events)
                .Include(x=>x.Certificates)
                    .ThenInclude(x=>x.Certificate)
                 .Include(c=>c.Certificates)   
                    .ThenInclude(c=>c.VerifiedBy)
                .Include(a=>a.AccountTransactions)
                .FirstOrDefaultAsync(x => x.Id == id)
                
                ;

            if (user == null)
                return NotFound(new NotFoundObjectResult($"User with id '{id}' wasn't found in the database"));

            var userroles = um.GetRolesAsync(user);

            var editUser = new EditUserViewModel()
            {
                Roles = await rm.Roles.ToListAsync(),
                User = user,
                Userroles = new HashSet<string>(await db.UserRoles.Where(x => x.UserId == id).Select(x=>x.RoleId).ToListAsync()),
            };

           
            return View(editUser);
        }

        public async Task<ActionResult> Delete(string id)
        {
            var user = await db.ApplicationUsers.FindAsync(id);
            user.LockoutEnabled = true;
            user.LockoutEnd = new DateTime(9999, 12, 31);

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransaction(CreateUserAccountTransactionViewModel model)
        {
            model.Entry.PostingDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                StatusMessage = "Der er sket en fejl.";
                return RedirectToAction("Index", "Home");
            }

            var user = await db.ApplicationUsers.FindAsync(model.SelectedUserId);
            if (user == null)
                return NotFound();

            model.Entry.ApplicationUser = user;
            db.ApplicationUserAccountEntry.Add(model.Entry);

            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }

        [HttpPost,ActionName("MakeDeposit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransaction(string userid, decimal amount, string invoicenumber, AccountTypeEnum accounttype)
        {
            var entry = new ApplicationUserAccountEntry
            {
                Amount = amount,
                AccountType = accounttype,
                Description = $"Beløb Indsat af '{User.Identity.Name}'",
                InvoiceNumber = invoicenumber,
                PostingDate = DateTime.Now
            };

            var user = await db.ApplicationUsers.FindAsync(userid);
            if (user == null)
                return NotFound();

            entry.ApplicationUser = user;
            db.ApplicationUserAccountEntry.Add(entry);

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", "User", new { id = userid });
        }

        [HttpGet]
        public async Task<JsonResult> GetMembers()
        {
            var members = await um.GetUsersInRoleAsync(Userroles.Member);
            return Json(await db.ApplicationUsers.Where(x => members.Any(s => x.Id == s.Id)).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(EditUserViewModel model)
        {
            var user = await db.ApplicationUsers.FindAsync(model.User.Id);

            user.PhoneNumber = model.User.PhoneNumber;
            user.Email = model.User.Email;
            user.AccountNumber = model.User.AccountNumber;
            user.Firstname = model.User.Firstname;
            user.Lastname = model.User.Lastname;
            user.Streetaddress = model.User.Streetaddress;
            user.PostalCode = model.User.PostalCode;
            user.City = model.User.City;
            user.Country = model.User.Country;

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public async Task<IActionResult> Deactivate(string id)
        {
            var user = await db.ApplicationUsers.FindAsync(id);
            user.LockoutEnabled = true;
            user.LockoutEnd = new DateTime(9999, 12, 31);

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id });

        }

        public async Task<IActionResult> Reactivate(string id)
        {
            var user = await db.ApplicationUsers.FindAsync(id);
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now;

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id });

        }

        public async Task<IActionResult> RemoveFromRole(string userid, string roleid)
        {
            var user = await db.ApplicationUsers.FindAsync(userid);

            if (user != null)
                await um.RemoveFromRoleAsync(user, roleid);

            return RedirectToAction("Edit", new { id = userid });
        }

        public async Task<IActionResult> AddToRole(string userid, string roleid)
        {
            var user = await db.ApplicationUsers.FindAsync(userid);
            if (user != null)
                await um.AddToRoleAsync(user, roleid);

            return RedirectToAction("Edit", new { id = userid });
        }
        public async Task<IActionResult> VerifyCertificate (int id)
        {
            var certificate = await db.UserCertificates.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id == id);
            if (certificate == null)
                return NotFound();

            certificate.IsVerified = true;
            certificate.VerifiedDate = DateTime.Now;
            certificate.VerifiedBy = await db.ApplicationUsers.FindAsync(User.GetIdentityId());

            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = certificate.User.Id });
        }

        public IActionResult ShowTransactions(string userId, AccountTypeEnum accountType)
        {
            var pageModel = new UserAccountTransactions { AccountType = accountType, userId = userId };
            return PartialView("_AccountDetails", pageModel);
        }

        public async Task<JsonResult> _GetTransActions(string userId, AccountTypeEnum accountType)
        {
            var data = await db.ApplicationUsers
                .Include(x => x.AccountTransactions)
                .FirstOrDefaultAsync(x => x.Id == userId);

            var transactions = data.AccountTransactions
                .Where(x => x.AccountType == accountType)
                .Select(x=> new
                {
                    PostingDate = x.PostingDate.GetValueOrDefault().ToShortDateString(),
                    x.Description,
                    Amount = x.Amount.ToString(),
                    x.InvoiceNumber,
                });

            return new JsonResult(transactions, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,

            });

            //return JsonConvert.SerializeObject(transactions, Formatting.Indented);

        }
    }
}