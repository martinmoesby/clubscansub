using Clubscansub.BlazorApp.Components.Account;
using Clubscansub.BlazorApp.Components.Member;
using ClubScansub.Models.DTO;
using ClubScansub.Models.ViewModels;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.ComponentModel.Design.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Members : ComponentBase
    {
        [Inject]
        NotificationService notificationService { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }

        [Inject]
        MemberService memberService { get; set; }

        MemberDTO member = new();

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            await getMembersByRoleFilter();

        }

        private IList<MemberDTO> members;
        private bool isLoading = false;
        private string[] roleFilter = new string[] { Userroles.Administrator, Userroles.Divepro, Userroles.Divepro };

        private DialogOptions editDialogOptions = new DialogOptions()
        {
            Width = "800px",
            AutoFocusFirstElement = true,
            CloseDialogOnEsc = true,
        };

        private async Task setRoleFilter(string[] newFilter)
        {
            roleFilter = newFilter;
            await getMembersByRoleFilter();
        }

        private async Task getMembersByRoleFilter()
        {
            isLoading = true;

            members = (await userService.GetUsersByRolesAsync(roleFilter)).OrderBy(x => x.UserName).ToList();

            isLoading = false;
            StateHasChanged();
        }

        void onRowDblCLick(DataGridRowMouseEventArgs<MemberDTO> arg)
        {

            //Console.WriteLine("Trying to Show memberdata");
            dialogService.Open<MemberDetailsComponent>($"Edit member",
                new Dictionary<string, object>
                {
                    { "Member", arg.Data },
                    { "UpdateUserCallback", EventCallback.Factory.Create<MemberDTO>(this, updateMember) },
                    { "DeleteUserCallback", EventCallback.Factory.Create<MemberDTO>(this, deleteMember) }
                }, editDialogOptions);

        }

        void onCreateNewMemberClick()
        {
            dialogService.Open<RegisterUserComponent>($"Create new member",
                new Dictionary<string, object>
                {
                    { "OnRegisterSuccess", EventCallback.Factory.Create<RegisterUserResult>(this, createMember) },
                }, editDialogOptions);
        }

        async void deleteMember(MemberDTO member)
        {
            var deleteUser = await dialogService.Confirm($"Dou you want to delete {member.Name} ? This is irreverible and cannot be undone!", "Delete member", new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Yes" });
            if (deleteUser.GetValueOrDefault())
            {
                await memberService.DeleteAsync(member);
                dialogService.Close();
                await getMembersByRoleFilter();

            }

        }
        async void updateMember(MemberDTO model)
        {

            try
            {
                await memberService.UpdateAsync(model);
                await userService.SetRolesByUserId(model);
                dialogService.Close();
                notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2500, Summary = $"'{model.Name}' has been updated", CloseOnClick = true });
                await getMembersByRoleFilter();

            }
            catch (Exception ex)
            {
                await dialogService.Alert($"An error occurred: {ex.Message}", "Update error!", new AlertOptions() { OkButtonText = "OK" });
            }

        }
        async void createMember(RegisterUserResult registerUserResult)
        {
            await dialogService.Alert($"Member '{registerUserResult.User.Name}' was created.", "New user created");
            dialogService.Close();
            await getMembersByRoleFilter();
        }

    }
}
