using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Member
{
    public partial class AccountTansactionComponent : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public IList<ApplicationUserAccountEntry> Transactions { get; set; } = [];

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private void closeDialog()
        {
            dialogService.Close();
        }

        private AccountTypeEnum accountType { get; set; } = AccountTypeEnum.EventAccountType;
    }
}
