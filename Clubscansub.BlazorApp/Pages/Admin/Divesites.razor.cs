using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Drawing;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Divesites : ComponentBase
    {
        [Inject]
        ContextMenuService contextMenuService { get; set; }
        [Inject]
        NotificationService notificationService { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        SiteService siteService { get; set; }

        private IList<Divelocation> sites = new List<Divelocation>();
        IList<Divelocation> selectedSites = new List<Divelocation>();
        private bool isLoading = false;
        private DiveTypeEnum[] typeFilter = [DiveTypeEnum.TechDive, DiveTypeEnum.NatureDive, DiveTypeEnum.WreckDive];
        private string searchFilter = "";

        private RadzenDataGrid<Divelocation> grid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await getSitesByTypeFilter();
                StateHasChanged();
            }

        }

        private async Task getSitesByTypeFilter()
        {
            isLoading = true;

            sites = (await siteService.GetAllAsync()).Where(x => typeFilter.Contains(x.DiveType) && x.Name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Name).ToList();

            isLoading = false;
            StateHasChanged();
        }

        private async void applySearch(string filter)
        {
            searchFilter = filter;
            await getSitesByTypeFilter();
        }

        private async void resetFilter()
        {
            searchFilter = "";
            await getSitesByTypeFilter();
        }

        private string getImageSrc(DivelocationImage image)
            {if (image != null)
            {
                return $"data:image;base64,{Convert.ToBase64String(image.ImageData)}";

            }
            return "/images/logo-login.png";
        }

        void onCellContextMenu(DataGridCellMouseEventArgs<Divelocation> args)
        {
            selectedSites = new List<Divelocation> { args.Data };

        }
    }
}
