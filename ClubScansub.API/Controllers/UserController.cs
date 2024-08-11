using ClubScansub.Data;
using ClubScansub.Extensions;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
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
                accountBalance = user.Balance,
                profile = new
                {
                    user.Firstname,
                    user.Lastname,
                    user.Streetaddress,
                    user.PostalCode,
                    user.City,
                    user.Country,
                    user.PhoneNumber,
                    user.Email,
                    user.DayOfbirth,
                    user.UserName,
                    user.AccountNumber
                }
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
                return Unauthorized();
            var userInfo = new
            {
                username = $"{user.Firstname} {user.Lastname}",
                isSignedIn = true,
                isPremium = await userManager.IsInRoleAsync(user, Userroles.Member),
                isDivePro = await userManager.IsInRoleAsync(user, Userroles.Divepro),
                accountBalance = user.Balance,
                profile = new {
                    user.Firstname,
                    user.Lastname,
                    user.Streetaddress,
                    user.PostalCode,
                    user.City,
                    user.Country,
                    user.PhoneNumber,
                    user.Email,
                    DayOfbirth = user.DayOfbirth.ToLocalTime(),
                    user.UserName,
                    user.AccountNumber
                }

            };

            return Json(userInfo);

        }

        [Authorize]
        [Route("myprofile")]
        [HttpPost]
        public async Task<ActionResult> UpdateUser ([FromBody] Models.ApplicationUser model)
        {

            if (!TryValidateModel(model))
                return NotFound(model);
            
            var dbUser = await db.ApplicationUsers.FindAsync(User.GetIdentityId());
            
            if (dbUser == null)
                return Unauthorized();

            dbUser.Firstname = model.Firstname;
            dbUser.Lastname = model.Lastname;
            dbUser.Streetaddress = model.Streetaddress;
            dbUser.City = model.City;
            dbUser.Country = model.Country;
            dbUser.PostalCode = model.PostalCode;
            dbUser.DayOfbirth = model.DayOfbirth.ToLocalTime();

            await db.SaveChangesAsync();

            if (dbUser.Email != model.Email)
            {
                await userManager.SetEmailAsync(dbUser, model.Email);
            }

            if (dbUser.PhoneNumber != model.PhoneNumber)
            {
                await userManager.SetPhoneNumberAsync(dbUser, model.PhoneNumber);
            }

            await db.SaveChangesAsync();

            return await RefreshUserInfo();
        }

        [Authorize]
        [Route("mycertificates")]
        [HttpGet]
        public async Task<ActionResult> GetCertificates()
        {
            var user = await db.ApplicationUsers.Include(x => x.AccountTransactions).SingleAsync(x => x.Id == userManager.GetUserId(User));

            if (user == null)
                return Unauthorized();

            var certificates = db.UserCertificates.Where(x => x.User == user).Select(x=> new { 
                x.Id,
                x.FrontSideImage,
                x.BackSideImage,
                x.IssuedDate,
                x.IsVerified,
                CertificateName = x.Certificate.Name,
                CertificateShortNamt = x.Certificate.ShortName,
                CertifiedDepth = x.Certificate.DepthLimit,
                VerifiedByName = x.VerifiedBy.Name,
                x.VerifiedDate
            });

            return Json(certificates);

        }

        public class UpdateUserModel
        {
            public string City { get; set;}
            public string Country { get; set; }
            public string Email { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string PhoneNumber { get; set; }
            public string PostalCode { get; set; }
            public string StreetAddress { get; set; }

          //"city": "ALBERTSLUND",
          //"country": null,
          //"email": "martin@martinmoesby.com",
          //"firstname": "Martin Moesby",
          //"lastname": "Petersen",
          //"phonenumber": "42790168",
          //"postalCode": "2620",
          //"streetaddress": "Støvlestræde 19",
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt2Key"]));
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
