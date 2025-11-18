using ClubScansub.Blazor.UIComponents.Account;
using ClubScansub.Models;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Common
{
    public partial class EventUserActionsComponent :ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        MemberService memberService { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public bool ShowAddNewUser { get; set; } = false;

        [Parameter]
        public EventCallback<string> AddExistingUser { get; set; }

        [Parameter]
        public EventCallback<RegisterUserResult> AddNewUser { get; set; }


        private string selectedUser;
        private IList<ApplicationUser> members;
        private IList<ApplicationUser> filteredUsers;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                members = await memberService.GetAllAsync();
                filteredUsers = members;
                await InvokeAsync(StateHasChanged);
            }

        }

        private void onLoadExistingUsers(LoadDataArgs args)
        {
            if (!string.IsNullOrEmpty(args.Filter))
            {
                filteredUsers = memberService.FindAll(args.Filter);
                InvokeAsync(StateHasChanged);
            }
        }

        private async Task addNewUser_Click()
        {
            var sideDialogOptions = new SideDialogOptions()
            {
                Width = "800px",
                Position = DialogPosition.Left,
                ShowClose = true
            };

            var dialogOptions = new DialogOptions()
            {
                Width = "95%",
                Height="95%",
                ShowClose = true
            };

            var dialogParameters = new Dictionary<string, object>()
            {
                {"OnRegisterSuccess", EventCallback.Factory.Create<RegisterUserResult>(this, addNewUserCallback) },
                {"ShowAlertOnError", false }
            };

            await dialogService.OpenAsync<RegisterUserComponent>("Create new User", dialogParameters, dialogOptions);
        }

        private void enrollExistingUserCallback()
        {
            AddExistingUser.InvokeAsync(selectedUser);
            filteredUsers = new List<ApplicationUser>();
            selectedUser = string.Empty;
        }

        private void reserveSpotCallback()
        {
            AddExistingUser.InvokeAsync("scansub");
        }

        private void addNewUserCallback(RegisterUserResult registerdUser)
        {
            AddNewUser.InvokeAsync(registerdUser);
        }
    }
}
