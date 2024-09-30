using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;

namespace ClubScansub.Blazor.UIComponents.Courses
{
    public partial class CourseSessionDetailsComponent : ComponentBase
    {
        [Parameter]
        public ICollection<CourseSession> Sessions { get; set; } = new List<CourseSession>();

        private Dictionary<CourseSessionTypeEnum, string> iconsDictionary = new Dictionary<CourseSessionTypeEnum, string>() {
            {    CourseSessionTypeEnum.PoolSession, IconsTextsAndColors.Icons.COURSE_SESSION_POOL },
            {    CourseSessionTypeEnum.AcademicSession, IconsTextsAndColors.Icons.COURSE_SESSION_ACADEMIC },
            {    CourseSessionTypeEnum.OpenWaterSession, IconsTextsAndColors.Icons.COURSE_SESSION_OW }
        }; 

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
