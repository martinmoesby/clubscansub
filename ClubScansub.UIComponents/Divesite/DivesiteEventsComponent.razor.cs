using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;

namespace ClubScansub.Blazor.UIComponents.Divesite
{
    public partial class DivesiteEventsComponent : ComponentBase
    {
        [Inject]
        public EventService eventService { get; set; }

        [Parameter]
        public int DivesiteId { get; set; }
        [Parameter]
        public bool ActiveEventsOnly { get; set; } = true;

        [Parameter]
        public bool CompletedEventsOnly { get; set; } = false;

        private IList<Event> events = new List<Event>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (ActiveEventsOnly)
                    events = await eventService.GetActiveEventsByDivesiteId(DivesiteId);
                else if (CompletedEventsOnly)
                    events = await eventService.GetCompletedEventsByDivesiteId(DivesiteId);
                StateHasChanged();
            }
        }
    }
}
