using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Net.Http;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ClubScansub.BlazorApp.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        CookieThemeService _cookieThemeService { get; set; }

        [Inject]
        AuthenticationStateProvider authStateProvider { get; set; }

        [Inject]
        private ThemeService themeService { get; set; }

        private MenuItemDisplayStyle menuItemDisplayStyle;
        private string[] menuWidths = ["0px;","50px", "130px"];
        private string selectedMenuWidth = string.Empty;

        string adminroles = $"{Userroles.Administrator},{Userroles.Owner}";
        //private MenuState menuState;

        private bool sidebarExpanded = false;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }


        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
            //menuState++;
            //if ((int)menuState > 2)
            //    menuState = 0;
            //setMenuState();
        }

        private string logoFilename = "";
        private bool isDarkTheme = false;
        //internal enum MenuState
        //{
        //    Hidden,
        //    IconAndText,
        //    IconOnly
        //}
        protected override async Task OnInitializedAsync()
        {
            menuItemDisplayStyle = MenuItemDisplayStyle.IconAndText;
            themeService.SetTheme("software");
            themeService.ThemeChanged += OnThemeChanged;
            setLogoFileName();
            sidebarExpanded = false;

        }

        private void OnThemeChanged()
        {

            setLogoFileName();
            StateHasChanged();
        }

        private void changeTheme(string value)
        {
            themeService.SetTheme(value);
        }

        private void setLogoFileName()
        {

            if (themeService.Theme.Contains("dark"))
            {
                isDarkTheme = true;
                logoFilename = "images/logo_new_dark.png";
            }
            else
            {
                isDarkTheme = false;
                logoFilename = "images/logo_new.png";
            }
        }

        private void setMenuState()
        {
            selectedMenuWidth = menuItemDisplayStyle == MenuItemDisplayStyle.Icon ? menuWidths[2] : menuWidths[1];
            menuItemDisplayStyle = menuItemDisplayStyle == MenuItemDisplayStyle.Icon ? MenuItemDisplayStyle.IconAndText : MenuItemDisplayStyle.Icon;

        }
    }
}
