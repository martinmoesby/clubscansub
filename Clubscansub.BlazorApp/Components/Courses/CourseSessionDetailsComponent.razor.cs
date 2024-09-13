using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Courses
{
    public partial class CourseSessionDetailsComponent : ComponentBase
    {
        [Parameter]
        public ICollection<CourseSession> Sessions { get; set; } = new List<CourseSession>();

        private Dictionary<CourseSessionTypeEnum, string> iconsDictionary = new Dictionary<CourseSessionTypeEnum, string>() {
            {    CourseSessionTypeEnum.PoolSession, "pool" },
            {    CourseSessionTypeEnum.AcademicSession, "school" },
            {    CourseSessionTypeEnum.OpenWaterSession, "scuba_diving" }
        }; 

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
