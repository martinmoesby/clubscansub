using ClubScansub.Models.DTO;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Member
{
    public partial class MemberDetailsComponent
    {
        [Parameter]
        public MemberDTO Member { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

        }
    }
}
