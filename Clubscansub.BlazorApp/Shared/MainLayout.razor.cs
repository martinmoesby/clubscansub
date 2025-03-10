using System.Net.Http;
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

namespace ClubScansub.BlazorApp.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        string adminroles = $"{Userroles.Administrator},{Userroles.Owner}";

        private bool sidebarExpanded = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
                
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }
    }
}
