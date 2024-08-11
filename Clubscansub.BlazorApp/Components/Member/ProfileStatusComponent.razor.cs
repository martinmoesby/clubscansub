using ClubScansub.Models;
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

        [Inject]
        TooltipService tooltipService { get; set; }

        [Parameter]
        public ApplicationUser Profile { get; set; }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            if (!Profile.IsActive)
            {
                profileIconStyle= IconStyle.Danger;
                profileStatusPopupText = "Profile Deactivated";
                profileIcon = "person_off";
            }

            if (!Profile.PhoneNumberConfirmed)
            {
                phoneIconStyle = IconStyle.Danger;
                phoneStatusPopupText = "Phone number has NOT been confirmed";
                phoneIcon = "phone_locked";
            }

            if (!Profile.EmailConfirmed)
            {
                emailIconStyle = IconStyle.Danger;
                emailStatusPopupText = "Email has been NOT confirmed";
                emailIcon = "mail_lock";
            }

        }
        private IconStyle phoneIconStyle = IconStyle.Success;
        private IconStyle emailIconStyle = IconStyle.Success;
        private IconStyle profileIconStyle = IconStyle.Success;

        private string phoneIcon = "phone";
        private string emailIcon = "mail";
        private string profileIcon = "person";

        private string phoneStatusPopupText = "Phone number has been confirmed";
        private string emailStatusPopupText = "Email has been confirmed";
        private string profileStatusPopupText ="Profile Active";

            //var roles = await service.GetRolesByUserId(Profile);
            //var orderedRoles =  fixedRoleOrder.Select(x=> roles.Contains(x) ? x : "-1").Where(x=>x != "-1").ToArray();
            //role = orderedRoles.FirstOrDefault();

            //switch (role)
            //{
            //    case Userroles.Owner:
            //        userRoleShade = Shade.Dark;
            //        userRoleBadgeStyle = BadgeStyle.Danger;
            //        break;
            //    case Userroles.Administrator:
            //        userRoleShade = Shade.Darker;
            //        userRoleBadgeStyle = BadgeStyle.Warning;
            //        break;
            //    case Userroles.Divepro:
            //        userRoleShade = Shade.Default;
            //        userRoleBadgeStyle = BadgeStyle.Success;
            //        break;
            //    case Userroles.Member:
            //        userRoleShade = Shade.Lighter;
            //        userRoleBadgeStyle = BadgeStyle.Info;
            //        break;
            //    case Userroles.User:
            //        userRoleShade= Shade.Lighter;
            //        userRoleBadgeStyle = BadgeStyle.Secondary;
            //        break;
            //    case Userroles.Student:
            //        userRoleShade = Shade.Light;
            //        userRoleBadgeStyle = BadgeStyle.Secondary;
            //        break;
            //    default:
            //        break;
            //}
        

        //private string[] fixedRoleOrder = { Userroles.Owner, Userroles.Administrator , Userroles.Divepro, Userroles.Member, Userroles.User, Userroles.Student };
        //private string role { get; set; } = "No roles";
        //private Shade userRoleShade { get; set; } = Shade.Default;
        //private BadgeStyle userRoleBadgeStyle { get; set; } = BadgeStyle.Light;


        void ShowTooltip(ElementReference elementReference, string name )
        {
            if (name == "phone")
                tooltipService.Open(elementReference, phoneStatusPopupText, new TooltipOptions());
            if (name == "email")
                tooltipService.Open(elementReference, emailStatusPopupText, new TooltipOptions());
            if (name == "profile")
                tooltipService.Open(elementReference, profileStatusPopupText, new TooltipOptions());
        }
    }
}
