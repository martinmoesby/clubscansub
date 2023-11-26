using AutoMapper.Execution;
using Clubscansub.BlazorApp.Components.Account;
using Clubscansub.BlazorApp.Components.Member;
using ClubScansub.Models.DTO;
using ClubScansub.Models.ViewModels;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using Radzen;
using System;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Members : ComponentBase
    {
        [Inject]
        ContextMenuService contextMenuService { get; set; }

        [Inject]
        NotificationService notificationService { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }

        [Inject]
        MemberService memberService { get; set; }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            await getMembersByRoleFilter();

        }

        private IList<MemberDTO> members;
        IList<MemberDTO> selectedMembers;
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

            members = (await memberService.GetAllByRolesAsync(roleFilter)).OrderBy(x => x.UserName).ToList();

            isLoading = false;
            StateHasChanged();
        }

        void onRowDblCLick(DataGridRowMouseEventArgs<MemberDTO> arg)
        {

            //Console.WriteLine("Trying to Show memberdata");
            showEditDialog(arg.Data);

        }

        void onCreateNewMemberClick()
        {
            dialogService.Open<RegisterUserComponent>($"Create new member",
                new Dictionary<string, object>
                {
                    { "OnRegisterSuccess", EventCallback.Factory.Create<RegisterUserResult>(this, createMember) },
                }, editDialogOptions);
        }
        void onCellContextMenu(DataGridCellMouseEventArgs<MemberDTO> args)
        {
            selectedMembers = new List<MemberDTO>() { args.Data };
            var disableEnableMenuText = args.Data.LockoutEnd == DateTime.MaxValue ? "Activate" : "Deactivate";

            contextMenuService.Open(args,
                new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                new ContextMenuItem(){ Text = @disableEnableMenuText, Value = 2, Icon = "change_circle" },
                new ContextMenuItem(){ Text = "Top off account", Value = 3, Icon = "add" },
                },
                (e) => {
                    switch (e.Value)
                    {
                        case 1:
                            showEditDialog(args.Data);
                            break;
                        case 2:
                            toggleActiveStatus(args.Data);
                            break;
                        case 3:
                            showAddTransactionDialog(args.Data);
                            break;
                        default:
                            Console.WriteLine($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, MemberID: {args.Data.UserName}");
                            break;
                    }
                }
             );
        }


        private async void deleteMember(MemberDTO member)
        {
            var deleteUser = await dialogService.Confirm($"Dou you want to delete {member.Name} ? This is irreverible and cannot be undone!", "Delete member", new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Yes" });
            if (deleteUser.GetValueOrDefault())
            {
                await memberService.DeleteAsync(member);
                dialogService.Close();
                await getMembersByRoleFilter();

            }

        }
        private async void updateMember(MemberDTO model)
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
        private async void createMember(RegisterUserResult registerUserResult)
        {
            try
            {
                notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Duration = 2500,
                    Summary = "New user created",
                    Detail = $"Member '{registerUserResult.User.Name}' was created."
                });
                dialogService.Close();
                await getMembersByRoleFilter();
            }
            catch (Exception ex)
            {

                await dialogService.Alert($"An error occurred: {ex.Message}", "Registration error!", new AlertOptions() { OkButtonText = "OK" });
            }
        }
        private async void makeTransaction(AccountTransactionDTO transaction)
        {
            try
            {
                var updateMember = await memberService.AddTransactionAsync(transaction.CurrentMember, transaction);
                dialogService.Close();
                members.Where(x => x.Id == updateMember.Id).First().AccountTransactions = updateMember.AccountTransactions;
                StateHasChanged();

                notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2500, Summary = $"'{transaction.CurrentMember.Name}' has receiced a deposit of {transaction.Amount.ToString("C")}", CloseOnClick = true });
            }
            catch (Exception ex)
            {
                await dialogService.Alert($"An error occurred: {ex.Message}", "Transaction error!", new AlertOptions() { OkButtonText = "OK" });
            }
        }


        private void showEditDialog(MemberDTO member)
        {
            if (member == null)
            {
                return;
            }
            dialogService.Open<MemberDetailsComponent>($"Edit member",
                new Dictionary<string, object>
                {
                                { "Member", member },
                                { "UpdateUserCallback", EventCallback.Factory.Create<MemberDTO>(this, updateMember) },
                                { "DeleteUserCallback", EventCallback.Factory.Create<MemberDTO>(this, deleteMember) }
                }, editDialogOptions);
        }
        private async void toggleActiveStatus(MemberDTO member)
        {
            var disableEnableMenuText = member.LockoutEnd == DateTime.MaxValue ? "Activate" : "Deactivate";
            var confirmAnswer = await dialogService.Confirm($"Do you want to {disableEnableMenuText} '{member.Name}'? ", disableEnableMenuText, new ConfirmOptions() { CloseDialogOnEsc = true, OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmAnswer.GetValueOrDefault())
            {
                member = await memberService.ToggleActiveStatus(member);
                members.Where(x => x.Id == member.Id).First().LockoutEnd = member.LockoutEnd;
                StateHasChanged();
            }
        }
        private void showAddTransactionDialog(MemberDTO member)
        {
            if (member == null) return;

            dialogService.Open<AddToAccountBalance>("Add transaction",
                new Dictionary<string, object>
                {
                     { "Member", member },
                     { "Callback", EventCallback.Factory.Create<AccountTransactionDTO>(this, makeTransaction) },
                }
                , editDialogOptions);
        }
    }
}
