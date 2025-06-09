using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Exceptions;
using ClubScansub.Service.Interfaces;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace ClubScansub.Service
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IUserEmailStore<ApplicationUser> emailStore;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly HttpContext httpContext;
        private readonly NavigationManager navManager;
        private readonly ILogger<UserService> logger;

        public UserService(IOptions<ServiceOptions> options,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IUserStore<ApplicationUser> userStore,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            NavigationManager navigationManager,
            ILogger<UserService> logger
            ) : base(options, emailSender)
        {
            this.userStore = userStore;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContext = httpContextAccessor.HttpContext!;
            this.navManager = navigationManager;
            this.logger = logger;
            emailStore = getEmailStore();
            appDbContext = new Data.ApplicationDbContext(dbContextOptions);
        }
        public Task<IdentityResult> ChangePassordAsync(ApplicationUser user, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> EmailConfirmAsync(Guid userid, string code)
        {
            var user = await userManager.FindByIdAsync(userid.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new UserNotFoundIdentityError(userid.ToString()));
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            return await userManager.ConfirmEmailAsync(user, code);

        }

        public Task<List<AuthenticationScheme>> GetExternalLoginsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetUserAsync(Guid Id)
        {
            throw new NotImplementedException();
            
        }

        public Task<ApplicationUser> GetUserAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<SignInResult> LoginUserAsync(LoginUserDTO<ApplicationUser> model)
        {
            if (!model.Username.EndsWith("@scansub.dk"))
            {
                model.Username = $"{model.Username}@scansub.dk";
            }

            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                throw new UserNotFoundException(model.Username);
            }

            var cansignInResult =  await signInManager.CanSignInAsync(user); ;
            if (cansignInResult)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var isInRolesResult = await userManager.GetRolesAsync(user);
                    if (isInRolesResult.Count() == 0)
                    {
                        var addtoNonMembeRoleResult = await userManager.AddToRoleAsync(user, Userroles.User);
                        if (!addtoNonMembeRoleResult.Succeeded)
                        {
                            return SignInResult.Failed;
                        }
                    }
                    return SignInResult.Success;
                }

                return SignInResult.Failed;
                //var is2FAEnabled = await signInManager.IsTwoFactorClientRememberedAsync(user);
                //if (!is2FAEnabled)
                //{

                //}
                //return SignInResult.TwoFactorRequired;
            }
            else
            {
                return SignInResult.LockedOut;
            }
        }

        public async Task<IList<string>> GetRolesByUserId(ApplicationUser member)
        {
            try
            {
                var user = await userStore.FindByIdAsync(member.Id, new CancellationToken());
                if (user != null) 
                { 
                    //var mapperConfig = new MemberMapperConfiguration().Configure();
                    //var map = new Mapper(mapperConfig);
                    //var user = map.Map<ApplicationUser>(member);
                    //appDbContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    //await Task.Delay(100);
                    var roles = await userManager.GetRolesAsync(user);
                return roles;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine($"Error getting roles for {member.Name} :{ex.Message}");
                Console.ResetColor();
                return new List<string>();
            }

        }

        public async Task SetRolesByUserId(ApplicationUser member)
        {
            if (member.Roles == null)
                throw new ArgumentNullException(nameof(member.Roles));

            var user = await userStore.FindByIdAsync(member.Id, new CancellationToken());
            if (user == null)
                throw new ArgumentException($"User '{member.Id}' could not be retrieved");

            try
            {
                var existingRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, existingRoles);
                await userManager.AddToRolesAsync(user, member.Roles);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to reset users roles association.Se InnerExceptions for more information", ex);
            }

        }

        public async Task<RegisterUserResult> RegisterNewUserAsync(RegisterUserDTO userWM, string[] roles = null)
        {
            if (roles == null)
                roles = new string[] { Userroles.User };


            try
            {
                logger.LogInformation($"Initiating to create user '{userWM.Email}'");

                var user = createUser(userWM);
                //var returnUrl = generateUrl("/index", null);

                await userStore.SetUserNameAsync(user, userWM.Username, CancellationToken.None);
                await emailStore.SetEmailAsync(user, userWM.Email, CancellationToken.None);

                var randomPassword = PasswordGenerator.GetRandomAlphanumericString(10);
                var result = await userManager.CreateAsync(user, randomPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = generateCallbackUrl("/account/confirmemail", userId, code);

                    var passwordChangeToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    var changePasswordCallbackUrl = generateCallbackUrl("/account/changepassword", userId, passwordChangeToken);


                    await emailSender.SendEmailAsync(userWM.Email, "Please confirm your email for clubscansub.dk",
                         $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. For security reasons your password will be sendt in a seperate email");

                    await emailSender.SendEmailAsync(userWM.Email, "Your password for ClubScnsub.dk",
                        $"Please use this password to login to CLubscansub.dk : {randomPassword}. We STRONGLY suggest that you immediately login to the site and <a href='{HtmlEncoder.Default.Encode(changePasswordCallbackUrl)}'> change your password by clicking here</a>");

                    return new RegisterUserResult(user, requireConfirmedAccount);
                }
                else
                {
                    logger.LogWarning($"User '{userWM.Email}' NOT created");
                    var registerUserResult = new RegisterUserResult();
                    foreach (var error in result.Errors)
                    {
                        registerUserResult.AddError(error.Description);
                    }

                    return registerUserResult;
                }
            }
            catch (Exception ex)
            {
                var result = new RegisterUserResult();
                result.AddError($"Usercreation ´{userWM.Email}' failed with '{ex.Message}'");
                return result;
            }

        }

        public Task<ApplicationUser> UpdateUserAsync(ApplicationUser newUser)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ApplicationUser>> GetUsersByRolesAsync(params string[] roles)
        {
            List<ApplicationUser> users = new();
            foreach (var item in roles)
            {
                var roleUsers = await userManager.GetUsersInRoleAsync(item);
                users.AddRange(roleUsers);
                users = users.Distinct().ToList(); ;
            }

            return users;
        }

        #region "Private support functions"

        private bool requireConfirmedAccount => userManager.Options.SignIn.RequireConfirmedAccount;
        private IUserEmailStore<ApplicationUser> getEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }
        private ApplicationUser createUser(RegisterUserDTO userInfo)
        {
            try
            {
                var user = Activator.CreateInstance<ApplicationUser>();
                user.Firstname = userInfo.Firstname;
                user.Lastname = userInfo.Lastname;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.Streetaddress = userInfo.Street;
                user.PostalCode = userInfo.PostalCode;
                user.City = userInfo.City;
                user.Country = userInfo.Country;
                user.AccountNumber = userInfo.AccountNumber;

                return user;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private string generateCallbackUrl(string page, string userid, string code)
        {
            IReadOnlyDictionary<string, object?> values = new Dictionary<string, object?>()
            {
                { "userid", userid },
                { "code", code }
            };

            var uri = navManager.GetUriWithQueryParameters(navManager.ToAbsoluteUri(page).AbsoluteUri, values);
            return uri;

        }
        #endregion

        public class PasswordChangeIdentityError : IdentityError
        {
            public PasswordChangeIdentityError(string userid, string errorMessage)
            {
                Code = "Password change failed";
                Description = $"Unable to change password for user '{userid}': {errorMessage}";
            }
        }
        public class PasswordNotFoundIdentityError : IdentityError
        {
            public PasswordNotFoundIdentityError(string userid)
            {

                Code = "Password not found";
                Description = $"User '{userid}' does not have a local password. Set a local password first";
            }
        }
        public class UserNotFoundIdentityError : IdentityError
        {
            public UserNotFoundIdentityError(string id)
            {
                Code = "User not found";
                Description = $"User with id '{id}' was not found";
            }
        }


        #region Obsolete functions
        [Obsolete("This function cannot be used in a BLAZER app, becuase it changes the httpCOntext.")]
        public async Task<LoginResult> LoginUserAsync(string username, string password, bool persistent)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(username, password, persistent, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var loginResult = new LoginResult(true);
                    loginResult.UseMFA = result.RequiresTwoFactor;
                    return loginResult;
                }
                var returnResult = new LoginResult(false);
                if (result.IsNotAllowed)
                {
                    returnResult.AddError("NotAllowed");
                }

                if (result.IsLockedOut)
                {
                    returnResult.AddError("Account is locked out");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                var loginResult = new LoginResult(false);
                loginResult.AddError(ex.Message);
                return loginResult;
                //throw ex;
            }
        }

        [Obsolete("This function cannot be used in a BLAZER app, becuase it changes the httpCOntext.")]
        public async Task LoginUserAsync(ApplicationUser user, bool isPersistent)
        {
            await signInManager.SignInAsync(user, isPersistent);
        }

        #endregion
    }
}
