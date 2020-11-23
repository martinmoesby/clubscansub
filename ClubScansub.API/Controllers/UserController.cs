using ClubScansub.Data;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.API
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signinManager;
        private readonly IConfiguration config;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinManager, IConfiguration config)
            : base(db)
        {
            this.userManager = userManager;
            this.signinManager = signinManager;
            this.config = config;
        }

        //[RequireHttps]
        [Route("signin")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var signin = await signinManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!signin.Succeeded)
            {
                return Unauthorized("INVALID_CREDENTIALS");
            }

            var appUser = userManager.Users.Single(r => r.UserName == model.Email);
            var user = await db.ApplicationUsers.Include(x=>x.AccountTransactions).SingleAsync(x=>x.Id == appUser.Id);

            var token = GenerateJwtToken(model.Email, appUser);

            var userInfo = new
            {
                username = $"{user.Firstname} {user.Lastname}",
                isSignedIn = true,
                isPremium = await userManager.IsInRoleAsync(appUser, Userroles.Member),
                isDivePro = await userManager.IsInRoleAsync(appUser, Userroles.Divepro),
                accountBalance = user.Balance
            };

            return Json(new { token, userInfo });

        }


        [Authorize]
        [Route("myprofile")]
        [HttpGet]
        public async Task<ActionResult> RefreshUserInfo()
        {
            var user = await db.ApplicationUsers.Include(x => x.AccountTransactions).SingleAsync(x => x.Id == userManager.GetUserId(User));
            
            if (user == null)
                return NotFound();

            var userInfo = new
            {
                username = $"{user.Firstname} {user.Lastname}",
                isSignedIn = true,
                isPremium = await userManager.IsInRoleAsync(user, Userroles.Member),
                isDivePro = await userManager.IsInRoleAsync(user, Userroles.Divepro),
                accountBalance = user.Balance
            };

            return Json(userInfo);
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        private object GenerateJwtToken(string email, IdentityUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.FamilyName, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.Id),
                new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim("IsPremium", userManager.IsInRoleAsync(appUser, Userroles.Member).GetAwaiter().GetResult().ToString()),
                new Claim("IsDivePro", userManager.IsInRoleAsync(appUser, Userroles.Divepro).GetAwaiter().GetResult().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(config["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                    config["JwtIssuer"],
                    config["JwtIssuer"],
                    claims,
                    null,
                    expires,
                    creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
