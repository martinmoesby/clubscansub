using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service.ServiceResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> ChangePassordAsync(ApplicationUser user, string oldPassword, string newPassword);
        Task<IdentityResult> EmailConfirmAsync(Guid userid, string code);
        Task<List<AuthenticationScheme>> GetExternalLoginsAsync();
        Task<ApplicationUser> GetUserAsync(Guid Id);
        Task<ApplicationUser> GetUserAsync(string username);
        Task LoginUserAsync(ApplicationUser user, bool isPersistent);
        Task<SignInResult> LoginUserAsync(LoginUserDTO<ApplicationUser> model);
        Task<LoginResult> LoginUserAsync(string username, string password, bool persistent);
        Task<RegisterUserResult> RegisterNewUserAsync(RegisterUserDTO userWM, string[] userRoles);
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
        Task<IList<ApplicationUser>> GetUsersByRolesAsync(params string[] roles);
    }
}
