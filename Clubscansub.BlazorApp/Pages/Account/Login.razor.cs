using Clubscansub.BlazorApp.Components.Account;
using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Service.Exceptions;
using ClubScansub.Service.ServiceResults;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace Clubscansub.BlazorApp.Pages.Account
{
    public partial class Login
    {

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }

        [Parameter]
        public string returnUrl { get; set; } = "";

        [Parameter]
        public EventCallback<ApplicationUser> OnLoginSuccessFull { get; set; }

        protected override async Task OnParametersSetAsync()
        {

            await base.OnParametersSetAsync();
            Console.WriteLine($"Return url: {returnUrl}");
        }

        private async Task onLogin(LoginArgs args)
        {
            model.Error = string.Empty;
            try
            {

                var returnString = string.IsNullOrEmpty(returnUrl) ? "" : $"&returnUrl= {returnUrl}";
                model.Username = args.Username;
                model.Password = args.Password;
                var loginResult = await userService.LoginUserAsync(model);
                if (loginResult == SignInResult.Success)
                {
                    Guid key = BlazorLoginMiddleware<ApplicationUser>.AnnounceLogin(model);
                    navigationManager.NavigateTo($"/account/login?key={key}{returnString}", true);
                }
                else if (loginResult == Microsoft.AspNetCore.Identity.SignInResult.LockedOut)
                {
                    model.Error = "Your account is blocked";
                }
                else
                {
                    model.Error = "Login failed. Check your password.";
                   
                }
            }
            catch(UserNotFoundException ex)
            {
                model.Error = $"Invalid user: { ex.Message}";
            }
            catch (Exception ex)
            {
                model.Error = $"Error: {ex.Message}";
            }

            if (!string.IsNullOrEmpty(model.Error ))
            {
                await dialogService.Alert(model.Error, "Login failed", new AlertOptions() { CloseDialogOnOverlayClick = true, OkButtonText = "OK" });
            }

        }

        private async Task onRegister()
        {
            await dialogService.OpenAsync<RegisterUserComponent>("Register new account",
                new Dictionary<string, object>()
                {
                    { "OnRegisterSuccess", EventCallback.Factory.Create<RegisterUserResult>(this,onRegistered) }
                },
                new DialogOptions() { Width = "960px", CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true }
                );
        }

        private async void onRegistered(RegisterUserResult result)
        {
            dialogService.Close();
            await dialogService.Alert($"User: '{result.User.UserName}' has been created", "Congrats", new AlertOptions() { OkButtonText = "ok"});

        }
        private string userName { get; set; } = "";
        private string password { get; set; } = "";
        protected LoginUserDTO<ApplicationUser> model = new LoginUserDTO<ApplicationUser>();


    }
}
