using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Scheduler
{
    public partial class CourseEventComponent : ComponentBase
    {
        [Inject]
        protected CourseService courseService { get; set; }

        [Parameter]
        public int Id { get; set; }

        Course Course { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Course = await courseService.GetAsync(Id.ToString());
        }
    }
}
