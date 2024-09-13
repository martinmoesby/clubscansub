using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text.Json;

namespace Clubscansub.BlazorApp.Components.Account
{
    public partial class RegisterUserComponent : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }
        
        [Parameter]
        public EventCallback<RegisterUserResult> OnRegisterSuccess { get; set; }

        [Parameter]
        public bool ShowAlertOnError { get; set; } = true;

        private RegisterUserDTO newUser { get; set; } = new();

        private string[] userRoles { get; set; } = new string[] { Userroles.User, Userroles.Student };

        private async void onSubmit(RegisterUserDTO registerUser)
        {
            Console.WriteLine($"Submit: {JsonSerializer.Serialize(registerUser, new JsonSerializerOptions() { WriteIndented = true })}");

            if (registerUser != null)
            {
                var registerResult = await userService.RegisterNewUserAsync(registerUser, userRoles);
                if (registerResult != null && registerResult.IsSucceesfull)
                {
                     await OnRegisterSuccess.InvokeAsync(registerResult);
                }

                if (registerResult != null && !registerResult.IsSucceesfull) 
                {
                    var errorMessage = registerResult.Errors.FirstOrDefault();
                    if (ShowAlertOnError)
                    {
                        await dialogService.Alert($"{errorMessage}", "Error", new AlertOptions() { CloseDialogOnEsc = true, OkButtonText = "OK - got it" });
                    } else
                    {
                        await OnRegisterSuccess.InvokeAsync(registerResult);
                    }
                }
            }
        }

        private async void invalidSubmit(FormInvalidSubmitEventArgs args)
        {
            await dialogService.Alert("IPlease check you information - and fill out all required fields", "Error", new AlertOptions() { CloseDialogOnEsc = true, OkButtonText = "Got it!" });
        }

        private void onSetUseroles(string[] roles)
        {
            userRoles= roles;
        }

    }
}
