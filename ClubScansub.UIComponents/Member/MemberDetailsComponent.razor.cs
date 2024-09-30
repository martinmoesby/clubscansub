using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Models.ViewModels;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Member
{
    public partial class MemberDetailsComponent : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }

        [Parameter]
        public EventCallback<ApplicationUser> UpdateUserCallback { get; set; }

        [Parameter]
        public EventCallback<ApplicationUser> DeleteUserCallback { get; set; }

        [Parameter]
        public ApplicationUser Member { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var userRoles = (await userService.GetRolesByUserId(Member)).ToArray();
            Member.Roles= userRoles;
        }

        private void onInvalidSubmit()
        {
            dialogService.Alert("Invlid information - please check all data and try again", "Invalid", new AlertOptions() { OkButtonText= "OK" });
        }
        private void deleteUserClick()
        {
            DeleteUserCallback.InvokeAsync(Member);
        }

        private void onSetUserRoles(string[] userRoles)
        {
            Member.Roles = userRoles;
        }

    }
}
