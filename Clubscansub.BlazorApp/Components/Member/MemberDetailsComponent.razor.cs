using ClubScansub.Models.DTO;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Clubscansub.BlazorApp.Components.Member
{
    public partial class MemberDetailsComponent
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public EventCallback<MemberDTO> UpdateUserCallback { get; set; }

        [Parameter]
        public EventCallback<MemberDTO> DeleteUserCallback { get; set; }

        [Parameter]
        public MemberDTO Member { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        private void onInvalidSubmit()
        {
            dialogService.Alert("Invlid information - please check all data and try again", "Invalid", new AlertOptions() { OkButtonText= "OK" });
        }
        private void deleteUserClick()
        {
            DeleteUserCallback.InvokeAsync(Member);
        }

    }
}
