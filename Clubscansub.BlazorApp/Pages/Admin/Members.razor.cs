using Clubscansub.BlazorApp.Components.Member;
using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.ComponentModel.Design.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Members
    {
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        UserService userService { get; set; }

        MemberDTO member = new();
        IList<MemberDTO> selectedMembers;

        protected override async Task OnInitializedAsync()
        {
            
            await base.OnInitializedAsync();
            await getMembersByRoleFilter();
            
        }

        private IList<MemberDTO> members;
        private bool isLoading = false;
        private bool showOwner = true;
        private bool showAdmin = true;
        private bool showDivepro = true;
        private bool showMember = true;
        private bool showUser = true;
        private bool showStudent = true;
        private Dictionary<string, bool> showRoles;
        private string[] roleFilter;
        private async Task getMembersByRoleFilter()
        {
            isLoading = true;

            showRoles = new Dictionary<string, bool> {
                { Userroles.Owner, showOwner } ,
                { Userroles.Administrator, showAdmin } ,
                { Userroles.Divepro,showDivepro} ,
                { Userroles.Member,showMember} ,
                { Userroles.User,showUser} ,
                { Userroles.Student ,showStudent}
            };

            roleFilter = showRoles.Where(x => x.Value).Select(x => x.Key).ToArray();

            members = (await userService.GetUsersByRolesAsync(roleFilter)).OrderBy(x=>x.UserName).ToList();
            StateHasChanged();
            isLoading = false;
        }

        void onRowSelect(MemberDTO member)
        {

            dialogService.Open<MemberDetailsComponent>($"Selected member: { member.Email} ", new Dictionary<string, object> { { "Member", member } });
        }
    }
}
