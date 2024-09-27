using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Common
{
    public partial class EventUserListComponent : ComponentBase
    {
        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public IList<EventUser> EventUsers { get; set; } = new List<EventUser>();

        [Parameter]
        public EventCallback<ApplicationUser> OnRemoveUser { get; set; }

        [Parameter]
        public EventCallback OnReleaseSpot { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; } = false;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
        private void removeUserFromEvent_Click(ApplicationUser user)
        {
            OnRemoveUser.InvokeAsync(user);
        }

        private void removeFixedUser()
        {
            OnReleaseSpot.InvokeAsync();

        }
    }
}
