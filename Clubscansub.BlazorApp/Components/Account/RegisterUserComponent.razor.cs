using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using Microsoft.AspNetCore.Components;
using Radzen;

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

        private RegisterUserDTO newUser { get; set; } = new();

        private async void onSubmit(RegisterUserDTO newUser)
        {
            if (newUser != null)
            {
                var registerResult =await  userService.RegisterNewUserAsync(newUser);
                if (registerResult != null && registerResult.IsSucceesfull)
                {
                    
                     await OnRegisterSuccess.InvokeAsync(registerResult);
                }

                if (registerResult != null && !registerResult.IsSucceesfull) 
                {
                    var errorMessage = registerResult.Errors.FirstOrDefault();

                    await dialogService.Alert($"{errorMessage}", "Error", new AlertOptions() { CloseDialogOnEsc = true, OkButtonText = "OK - got it" });
                }
            }
        }

        private async void invalidSubmit(FormInvalidSubmitEventArgs args)
        {
            await dialogService.Alert("InvalidCastException registration", "Error", new AlertOptions() { CloseDialogOnEsc = true });
        }


    }
}
