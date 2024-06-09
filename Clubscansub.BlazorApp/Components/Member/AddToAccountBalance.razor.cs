using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;

namespace Clubscansub.BlazorApp.Components.Member
{
    public partial class AddToAccountBalance : ComponentBase
    {
        [Inject]
        MemberService memberService { get; set; }

        [Parameter]
        public ApplicationUser Member { get; set; }

        [Parameter]
        public EventCallback<ApplicationUserAccountEntry> Callback{ get; set; }

        protected override void OnInitialized()
        {

        }

        private void saveTransaction()
        {
            transaction.ApplicationUser = Member;
            Callback.InvokeAsync(transaction);
        }

        private AccountTypeEnum accountType { get; set; } = AccountTypeEnum.EventAccountType;
        private ApplicationUserAccountEntry transaction { get; set; } = new();

    }
}
