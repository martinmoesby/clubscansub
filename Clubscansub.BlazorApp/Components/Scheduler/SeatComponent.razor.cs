using ClubScansub.Models;
using ClubScansub.Models.Interface;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Scheduler
{
    public partial class SeatComponent : ComponentBase
    {
        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public bool ShowText { get; set; } = true;

        [Parameter]
        public string Text { get; set; } = "Status";

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

        }
    }
}
