using Clubscansub.BlazorApp.Components.Account;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Service.ServiceResults;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Clubscansub.BlazorApp.Components.Courses
{
    public partial class CourseEventComponent : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        NotificationService notificationService { get; set; }

        [Inject]
        protected CourseService courseService { get; set; }

        [Inject]
        protected MemberService memberService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public int SessionId { get; set; }

        private bool isLoading = true;
        private Course course = new();
        private CourseSession session = new();

        private IList<EventUser> enrolledUsers = new List<EventUser>();
        private IEnumerable<ApplicationUser> filteredUsers = new List<ApplicationUser>();
        private string selectedUser = string.Empty;


        private string courseBrief = "";

        private async Task addNewStudent_Click()
        {
            var dialogOptions = new SideDialogOptions()
            {
                Width = "800px",
                Position = DialogPosition.Left,
                ShowClose = true
            };

            var dialogParameters = new Dictionary<string, object>()
            {
                {"OnRegisterSuccess", EventCallback.Factory.Create<RegisterUserResult>(this, addNewStudent) },
                {"ShowAlertOnError", false }
            };

            await dialogService.OpenSideAsync<RegisterUserComponent>("Add new student", dialogParameters, dialogOptions);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                course = await courseService.GetAsync(Id.ToString());
                session = course.CourseSessions.FirstOrDefault(x => x.Id == SessionId);
                enrolledUsers = await courseService.GetUsersByCourse(Id);
                isLoading = false;
                courseBrief = @$"I forløbet har vi  
                {course.CourseSessions.Count(x => x.Sessiontype == ClubScansub.Utility.CourseSessionTypeEnum.AcademicSession)} dag(e) med teori i klasselokale,
                {course.CourseSessions.Count(x => x.Sessiontype == ClubScansub.Utility.CourseSessionTypeEnum.PoolSession)} dag(e) med træning og gemmengang i svømmehal og
                {course.CourseSessions.Count(x => x.Sessiontype == ClubScansub.Utility.CourseSessionTypeEnum.OpenWaterSession)} dag(e) hvor træner i havet";
                StateHasChanged();
            }

        }

        private void OnLoadExistingUsers(LoadDataArgs args)
        {
            if (!string.IsNullOrEmpty(args.Filter))
            {
                filteredUsers = memberService.FindAll(args.Filter);
                InvokeAsync(StateHasChanged);
            }
        }

        private async Task OnLoadEnrolledUsers()
        {
            enrolledUsers = await courseService.GetUsersByCourse(Id);
            StateHasChanged();
        }

        private async Task enrollExistingUser()
        {
            if (enrolledUsers.Any(x => x.ApplicationUserId == selectedUser))
            {
                notificationService.Notify(NotificationSeverity.Warning, "This user is already enrolled to the course");

            }
            else
            {
                await courseService.EnrollUser(selectedUser, Id);
                selectedUser = string.Empty;
                await OnLoadEnrolledUsers();
                notificationService.Notify(NotificationSeverity.Success, $"User enrolled for the course");
            }
        }
        private async void removeUserFromCourse(ApplicationUser user)
        {
            var confirm = (await dialogService.Confirm($"Do you want to remove {user.Name} from the course?", "Remove user"));
            if (confirm.Value)
            {
                //TODO: Remove user from course and return any deposits made for the enrollment
                await courseService.RemoveUserFromCourse(user.Id, Id);
                await OnLoadEnrolledUsers();
            }
        }
        private async Task addNewStudent(RegisterUserResult registeredUserResult)
        {
            dialogService.CloseSide();
            if (registeredUserResult.IsSucceesfull)
            {
                await courseService.EnrollUser(registeredUserResult.User.Id, Id);
                await OnLoadEnrolledUsers();

                notificationService.Notify(NotificationSeverity.Success, $"{registeredUserResult.User.UserName} {registeredUserResult.User.Email} created and added submitted to the course");
            }
            else
            {
                var errorMessage = registeredUserResult.Errors[0];
                await dialogService.Alert($"Please fix the errors and try creating the user again or tryr to pick the user from the dropdown instead.", $"User was not created due to '{errorMessage}'", new AlertOptions()
                {

                });
            }
        }
    }
}
