using ClubScansub.Models.DTO;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Clubscansub.BlazorApp.Components.Member
{
    public partial class ProfileStatusComponent : ComponentBase
    {
        [Inject]
        UserService service { get; set; }

        [Parameter]
        public MemberDTO Profile { get; set; }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            phoneBadgeStyle = Profile.PhoneNumberConfirmed ? BadgeStyle.Success : BadgeStyle.Danger;
            emailBadgeStyle = Profile.EmailConfirmed ? BadgeStyle.Success: BadgeStyle.Danger;

            var roles = (await service.GetRolesByUserId(Profile)).ToArray();
            var orderedRoles =  fixedRoleOrder.Select(x=> roles.Contains(x) ? x : "-1").Where(x=>x != "-1").ToArray();
            role = orderedRoles.FirstOrDefault();

            switch (role)
            {
                case Userroles.Owner:
                    userRoleShade = Shade.Dark;
                    userRoleBadgeStyle = BadgeStyle.Danger;
                    break;
                case Userroles.Administrator:
                    userRoleShade = Shade.Darker;
                    userRoleBadgeStyle = BadgeStyle.Warning;
                    break;
                case Userroles.Divepro:
                    userRoleShade = Shade.Default;
                    userRoleBadgeStyle = BadgeStyle.Success;
                    break;
                case Userroles.Member:
                    userRoleShade = Shade.Lighter;
                    userRoleBadgeStyle = BadgeStyle.Info;
                    break;
                case Userroles.User:
                    userRoleShade= Shade.Lighter;
                    userRoleBadgeStyle = BadgeStyle.Secondary;
                    break;
                case Userroles.Student:
                    userRoleShade = Shade.Light;
                    userRoleBadgeStyle = BadgeStyle.Secondary;
                    break;
                default:
                    break;
            }
        }

        private string[] fixedRoleOrder = { Userroles.Owner, Userroles.Administrator , Userroles.Divepro, Userroles.Member, Userroles.User, Userroles.Student };
        private string role { get; set; } = "No roles";
        private BadgeStyle phoneBadgeStyle { get; set; }
        private BadgeStyle emailBadgeStyle { get; set; }

        private Shade userRoleShade { get; set; } = Shade.Default;
        private BadgeStyle userRoleBadgeStyle { get; set; } = BadgeStyle.Light;


    }
}
