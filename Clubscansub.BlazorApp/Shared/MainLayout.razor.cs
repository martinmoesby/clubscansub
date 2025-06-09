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

        string adminroles = $"{Userroles.Administrator},{Userroles.Owner}";

        private bool sidebarExpanded = false;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        private string logoFilename = "";
        private bool isDarkTheme = false;

        protected override async Task OnInitializedAsync()
        {

            sidebarExpanded = false;
            themeService.ThemeChanged += OnThemeChanged;
            setLogoFileName();
            var authState = await authStateProvider.GetAuthenticationStateAsync();

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

    }
}
