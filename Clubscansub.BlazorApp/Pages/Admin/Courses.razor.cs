using ClubScansub.Data.Migrations;
using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using ClubScansub.Utility;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;
using System.Diagnostics;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Courses : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        CourseService courseService { get; set; }

        [Inject]
        EventUserService eventUserService { get; set; }

        RadzenDataGrid<Course> activeCoursesGrid;
        
        private IList<Course> courses = new List<Course>();
        private IList<Course> completedCourses = new List<Course>();
        private IList<Course> filteredCompletedCourses = new List<Course>();
        private IList<CourseTemplate> templates = new List<CourseTemplate>();
        private IList<CourseTemplate> filteredTemplates = new List<CourseTemplate>();

        private bool showOnlyExecutedCourses = true;
        private string completedFilterString = "";

        //private Dictionary<CourseTypeEnum, bool> courseTypeFilters = new Dictionary<CourseTypeEnum, bool>() {
        //    {    CourseTypeEnum.BaseCourse, true },
        //    {    CourseTypeEnum.SpecialtyCourse, true },
        //    {    CourseTypeEnum.TechCourse, true },
        //    {    CourseTypeEnum.ProCourse, true }
        //};

        private Dictionary<CourseTypeEnum, bool> completedCourseTypeFilters = new Dictionary<CourseTypeEnum, bool>() {
            {    CourseTypeEnum.BaseCourse, true },
            {    CourseTypeEnum.SpecialtyCourse, true },
            {    CourseTypeEnum.TechCourse, true },
            {    CourseTypeEnum.ProCourse, true }
        };

        private CourseTypeEnum selectedCourseType = CourseTypeEnum.BaseCourse;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                courses = await courseService.GetAllAsync();
                completedCourses = await courseService.GetStartedOrCompletedAsync();
                filterCompletedCourses();
                templates = await courseService.GetTemplatesAsync();               
                applyTemplateFilter();
            }
        }
        private bool getCourseFilter(CourseTypeEnum type)
        {
            return completedCourseTypeFilters[type];
        }
        private string getCourseIcon(CourseTypeEnum item)
        {
            switch (item)
            {
                case CourseTypeEnum.BaseCourse:
                    return IconsTextsAndColors.Icons.EVENTTYPE_BASE_COURSE;
                case CourseTypeEnum.SpecialtyCourse:
                    return IconsTextsAndColors.Icons.EVENTTYPE_SPEC_COURSE;
                case CourseTypeEnum.TechCourse:
                    return IconsTextsAndColors.Icons.EVENTTYPE_TECH_COURSE;
                case CourseTypeEnum.ProCourse:
                    return IconsTextsAndColors.Icons.EVENTTYPE_PRO_COURSE;

            }
            return "";
        }
        private string getCourseSessionIcon(CourseSessionTypeEnum item)
        {
            switch (item)
            {
                case CourseSessionTypeEnum.AcademicSession:
                    return IconsTextsAndColors.Icons.COURSE_SESSION_ACADEMIC;
                case CourseSessionTypeEnum.PoolSession:
                    return IconsTextsAndColors.Icons.COURSE_SESSION_POOL;
                case CourseSessionTypeEnum.OpenWaterSession:
                    return IconsTextsAndColors.Icons.COURSE_SESSION_OW;
            }
            return "";
        }

        private async void setCourseFilter(CourseTypeEnum type)
        {
            completedCourseTypeFilters[type] = !completedCourseTypeFilters[type];
            filterCompletedCourses();
        }
        private void applyTemplateFilter()
        {
            filteredTemplates = templates.Where(x => x.CourseType == selectedCourseType).ToList();
            StateHasChanged();
        }

        private void filterCompletedCoursesByUser(string filter)
        {
            completedFilterString = filter.ToLower();
            filterCompletedCourses();
        }

        private void resetCompletedCoursesParticipantsFilter()
        {
            completedFilterString = string.Empty;
            filterCompletedCourses();
        }
        private void filterCompletedCourses()
        {
            var completedCoursesEventUsers = completedCourses.SelectMany(x => x.Participants);
            var completedCoursesFilteredEvents = completedCoursesEventUsers.Where(x => x.ApplicationUser.Name.ToLower().Contains(completedFilterString) || x.ApplicationUser.AccountNumber.Contains(completedFilterString)).Select(x=>x.EventId);

            filteredCompletedCourses = completedCourses.Where(x=> completedCoursesFilteredEvents.Contains(x.Id) || x.Participants.Count == 0).ToList();
            var completedCourseTypes = completedCourseTypeFilters.Where(x => x.Value == true).Select(x => x.Key);

            filteredCompletedCourses = filteredCompletedCourses.Where(x => completedCourseTypes.Contains(x.CourseType)).ToList();
            if (showOnlyExecutedCourses)
                filteredCompletedCourses = filteredCompletedCourses.Where(x => x.Participants.Count > 0).ToList();
            else
                filteredCompletedCourses = filteredCompletedCourses.Where(x => x.Participants.Count >= 0).ToList();

            StateHasChanged();
        }

        private async void addNewStudent(RegisterUserResult newUser, int CourseId)
        {
            dialogService.CloseSide();
            enrollExistingUser(newUser.User.Id, CourseId);
        }

        private async void enrollExistingUser(string selectedUserId, int CourseId)
        {
            var activecourse = courses.FirstOrDefault(x => x.Id == CourseId);

            //TODO: Remove user from course and return any deposits made for the enrollment
            activecourse.Participants = await eventUserService.AddUserToEvent(selectedUserId, CourseId);
            await InvokeAsync(StateHasChanged);
        }
        private async void removeUserFromCourse(ApplicationUser user, int CourseId)
        {
            var confirm = (await dialogService.Confirm($"Do you want to remove {user.Name} from the course?", "Remove user"));
            if (confirm.Value)
            {
                var activecourse = courses.FirstOrDefault(x => x.Id == CourseId);

                //TODO: Remove user from course and return any deposits made for the enrollment
                activecourse.Participants = await eventUserService.RemoveUserFromEvent(user.Id, CourseId);
                await InvokeAsync(StateHasChanged);
                
            }
        }


    }
}
