using ClubScansub.Models.DTO;
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
        public MemberDTO Member { get; set; }

        [Parameter]
        public EventCallback<AccountTransactionDTO> Callback{ get; set; }

        protected override void OnInitialized()
        {

        }

        private void saveTransaction()
        {
            transaction.CurrentMember = Member;
            Callback.InvokeAsync(transaction);
        }

        private AccountTypeEnum accountType { get; set; } = AccountTypeEnum.EventAccountType;
        private AccountTransactionDTO transaction { get; set; } = new();

    }
}
