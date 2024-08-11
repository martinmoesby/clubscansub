using Clubscansub.BlazorApp.Components.Account;
using Clubscansub.BlazorApp.Components.Member;
using ClubScansub.Models;
using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

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

        private RadzenDataGrid<ApplicationUser> grid { get; set; }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await getMembersByRoleFilter();
                StateHasChanged();
            }
        }

        private IList<ApplicationUser> members;
        IList<ApplicationUser> selectedMembers;
        private bool isLoading = false;
        private string[] roleFilter = new string[] { Userroles.Administrator, Userroles.Divepro, Userroles.Divepro };
        private string searchFilter = "";
        private bool showDeactivated = false;


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

        private async Task setStatusFilter(bool status) { 
        
            showDeactivated = status;
            await getMembersByRoleFilter();
        }

        private async Task getMembersByRoleFilter()
        {
            isLoading = true;

            members = (await memberService.GetAllByRolesAsync(roleFilter)).OrderBy(x => x.UserName).Where(x=> x.SearchStringValue.Contains(searchFilter,StringComparison.OrdinalIgnoreCase) && x.IsActive != showDeactivated).ToList();

            isLoading = false;
            StateHasChanged();
        }


        async void resetSearchFilter()
        {
            searchFilter = "";
            await getMembersByRoleFilter();
        }
        void onRowDblCLick(DataGridRowMouseEventArgs<ApplicationUser> arg)
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
        void onCellContextMenu(DataGridCellMouseEventArgs<ApplicationUser> args)
        {
            selectedMembers = new List<ApplicationUser>() { args.Data };
            var disableEnableMenuText = args.Data.LockoutEnd == DateTime.MaxValue ? "Activate" : "Deactivate";

            contextMenuService.Open(args,
                new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                new ContextMenuItem(){ Text = @disableEnableMenuText, Value = 2, Icon = "change_circle" },
                new ContextMenuItem(){ Text = "Top off account", Value = 3, Icon = "add" },
                new ContextMenuItem(){ Text = "Transactions", Value = 4, Icon = "receipt_long" },

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
                        case 4:
                            showTransactionsDialog(args.Data);
                            break;
                        default:
                            Console.WriteLine($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, MemberID: {args.Data.UserName}");
                            break;
                    }
                }
             );
        }


        private async void deleteMember(ApplicationUser member)
        {

            var deleteUser = await dialogService.Confirm($"Dou you want to delete {member.Name} ? This is irreverible and cannot be undone!", "Delete member", new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Yes" });
            if (deleteUser.GetValueOrDefault())
            {
                await memberService.DeleteAsync(member);
                dialogService.Close();

                notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Duration = 2500,
                    Summary = "Member deleted!",
                    Detail = $"Member '{member.Name}' was deleted."
                });

                await getMembersByRoleFilter();

            }

        }
        private async void updateMember(ApplicationUser model)
        {

            try
            {
                await userService.SetRolesByUserId(model);
                await memberService.UpdateAsync(model);
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
                if (registerUserResult.IsSucceesfull)
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

                } else
                {
                    notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Duration = 2500,
                        Summary = registerUserResult.Errors.FirstOrDefault(),
                        Detail =  string.Join(" - ", registerUserResult.Errors.ToArray())
                    });
                    dialogService.Close();

                }

            }
            catch (Exception ex)
            {

                await dialogService.Alert($"An error occurred: {ex.Message}", "Registration error!", new AlertOptions() { OkButtonText = "OK" });
            }
        }
        private async void makeTransaction(ApplicationUserAccountEntry transaction)
        {
            try
            {
                var updateMember = await memberService.AddTransactionAsync(transaction.ApplicationUser, transaction);
                dialogService.Close();
                members.Where(x => x.Id == updateMember.Id).First().AccountTransactions = updateMember.AccountTransactions;
                StateHasChanged();

                notificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2500, Summary = $"'{transaction.ApplicationUser.Name}' has receiced a deposit of {transaction.Amount.ToString("C")}", CloseOnClick = true });
            }
            catch (Exception ex)
            {
                await dialogService.Alert($"An error occurred: {ex.Message}", "Transaction error!", new AlertOptions() { OkButtonText = "OK" });
            }
        }
        private async void toggleActiveStatus(ApplicationUser member)
        {
            var disableEnableMenuText = member.LockoutEnd == DateTime.MaxValue ? "Activate" : "Deactivate";
            var confirmAnswer = await dialogService.Confirm($"Do you want to {disableEnableMenuText} '{member.Name}'? ", disableEnableMenuText, new ConfirmOptions() { CloseDialogOnEsc = true, OkButtonText = "Yes", CancelButtonText = "No" });
            if (confirmAnswer.GetValueOrDefault())
            {
                member = await memberService.ToggleActiveStatus(member);
                members.Where(x => x.Id == member.Id).First().LockoutEnd = member.LockoutEnd;
                StateHasChanged();
                await grid.RefreshDataAsync();
               // await getMembersByRoleFilter();
            }
        }

        private void showEditDialog(ApplicationUser member)
        {
            if (member == null)
            {
                return;
            }
            dialogService.Open<MemberDetailsComponent>($"Edit member",
                new Dictionary<string, object>
                {
                                { "Member", member },
                                { "UpdateUserCallback", EventCallback.Factory.Create<ApplicationUser>(this, updateMember) },
                                { "DeleteUserCallback", EventCallback.Factory.Create<ApplicationUser>(this, deleteMember) }
                }, editDialogOptions);
        }
        private void showAddTransactionDialog(ApplicationUser member)
        {
            if (member == null) return;

            dialogService.Open<AddToAccountBalance>("Add transaction",
                new Dictionary<string, object>
                {
                     { "Member", member },
                     { "Callback", EventCallback.Factory.Create<ApplicationUserAccountEntry>(this, makeTransaction) },
                }
                , editDialogOptions);
        }

        private void showTransactionsDialog(ApplicationUser member)
        {
            if (member == null) return;

            dialogService.Open<AccountTansactionComponent>("Transactions",
                new Dictionary<string, object>
                {
                     { "Transactions", member.AccountTransactions }
                }
                , editDialogOptions);
        }

        private void closeDialog()
        {
            dialogService.Close();
        }
    }
}
