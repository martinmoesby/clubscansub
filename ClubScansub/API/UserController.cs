using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClubScansub.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClubScansub.API
{
    [Route("api/[controller]")]
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

        [RequireHttps]
        [HttpPost]
        public async Task<ActionResult> login([FromBody] LoginModel model)
        {
            var signin = await signinManager.PasswordSignInAsync(model.Email, model.Password,false, false);
            if (!signin.Succeeded)
            {
                return Unauthorized("INVALID_CREDENTIALS");
            }

            var appUser = userManager.Users.Single(r => r.Email == model.Email);
            var token = GenerateJwtToken(model.Email, appUser);
            return Json(new { token });

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
                new Claim("IsPremium","true"),
                new Claim("IsDivePro","false")
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
