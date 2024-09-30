using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Member
{
    public partial class MemberRolesComponent :ComponentBase
    {
        [Parameter]
        public string LabelText { get; set; } = "Role filter";

        [Parameter]
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        [Parameter]
        public AlignItems Alignment { get; set; } = AlignItems.Stretch;

        [Parameter]
        public JustifyContent Justify { get; set; } = JustifyContent.Normal;

        [Parameter]
        public bool ShowLabel { get; set; } = true;
        
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public bool ShowStatusFilter { get; set; } = false;

        [Parameter]
        public string[] InitialRoles { get; set; }

        [Parameter]
        public EventCallback<string[]> MemberRolesChanged { get; set; }

        [Parameter]
        public EventCallback<bool> MemberStatusFilterChanged { get; set; }

        protected override Task OnInitializedAsync()
        {
            showAdmin = InitialRoles.Any(x => x.Equals(Userroles.Administrator));
            showOwner = InitialRoles.Any(x => x.Equals(Userroles.Owner));
            showDivepro = InitialRoles.Any(x => x.Equals(Userroles.Divepro));
            showMember = InitialRoles.Any(x => x.Equals(Userroles.Member));
            showUser = InitialRoles.Any(x => x.Equals(Userroles.User));
            showStudent = InitialRoles.Any(x => x.Equals(Userroles.Student));
            return base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            
        }

        private bool showOwner = false;
        private bool showAdmin = false;
        private bool showDivepro = true;
        private bool showMember = true;
        private bool showUser = true;
        private bool showStudent = false;

        private bool showDeactivated = false;

        private Dictionary<string, bool> showRoles;
        private string[] roleFilter;

        private void onMemberRolesChanged()
        {
            showRoles = new Dictionary<string, bool> {
                { Userroles.Owner, showOwner } ,
                { Userroles.Administrator, showAdmin } ,
                { Userroles.Divepro,showDivepro} ,
                { Userroles.Member,showMember} ,
                { Userroles.User,showUser} ,
                { Userroles.Student ,showStudent}
            };

            roleFilter = showRoles.Where(x => x.Value).Select(x => x.Key).ToArray();

            MemberRolesChanged.InvokeAsync(roleFilter);
        }

        private void onMemberStatusFilterChanged()
        {
            MemberStatusFilterChanged.InvokeAsync(showDeactivated);
        }
    }
}
