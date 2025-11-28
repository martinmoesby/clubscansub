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
using System.Reflection;

namespace ClubScansub.BlazorApp.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        //[Inject]
        //CookieThemeService _cookieThemeService { get; set; }

        [Inject]
        AuthenticationStateProvider authStateProvider { get; set; }

        [Inject]
        private ThemeService themeService { get; set; }

        private MenuItemDisplayStyle menuItemDisplayStyle;

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

        //internal enum MenuState
        //{
        //    Hidden,
        //    IconAndText,
        //    IconOnly
        //}

        private string versionFull = string.Empty;
        private string versionSmall = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            menuItemDisplayStyle = MenuItemDisplayStyle.IconAndText;
            themeService.ThemeChanged += OnThemeChanged;
            setLogoFileName();
            sidebarExpanded = false;
            //var V = Assembly.GetExecutingAssembly().GetName().Version;
            //versionFull = $"V{V.Major}.{V.Minor}.{V.MajorRevision}";
            //versionSmall = $"V{V.Major}.{V.Minor}";
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
                logoFilename = "images/logo_new_dark.png";
            }
            else
            {
                logoFilename = "images/logo_new.png";
            }
        }

        private void setMenuState()
        {
            menuItemDisplayStyle = menuItemDisplayStyle == MenuItemDisplayStyle.Icon ? MenuItemDisplayStyle.IconAndText : MenuItemDisplayStyle.Icon;

        }
    }
}
