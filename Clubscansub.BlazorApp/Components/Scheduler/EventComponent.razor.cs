using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Scheduler
{
    public partial class EventComponent : ComponentBase
    {
        [Inject]
        protected EventService eventService { get; set; }

        [Parameter]
        public int EventId { get; set; }

        Event Event { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var item = await eventService.GetAsync(EventId.ToString());
                if (item != null) { 
                    Event = item;
                }

                StateHasChanged();
            }
        }

    }
}
