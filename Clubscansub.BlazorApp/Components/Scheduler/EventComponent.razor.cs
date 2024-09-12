using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Clubscansub.BlazorApp.Components.Scheduler
{
    public partial class EventComponent : ComponentBase
    {
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }

        [Inject]
        protected EventService eventService { get; set; }

        [Inject]
        MemberService memberService { get; set; }

        [Parameter]
        public int EventId { get; set; }

        private ApplicationUser currentUser { get; set; } = new();
        Event Event { get; set; } = new Event();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                if (authState.User != null)
                {
                    var user = await memberService.GetAsync(authState.User.GetIdentityId());
                    currentUser = user;
                }
                var item = await eventService.GetAsync(EventId.ToString());
                if (item != null) { 
                    Event = item;
                }

                StateHasChanged();
            }
        }

    }
}
