using ClubScansub.Models;
using Microsoft.AspNetCore.Components;


namespace ClubScansub.Blazor.UIComponents.Common
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

        private bool readOnly { get; set; } = false;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                readOnly = Event.IsCancelled || Event.EndDateAndTime < DateTime.Now;
                StateHasChanged();
            }
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
