using Clubscansub.BlazorApp.Components.Divesite;
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

        private DialogOptions editDialogOptions = new DialogOptions()
        {
            Width = "800px",
            Height = "600px",
            AutoFocusFirstElement = true,
            CloseDialogOnEsc = true,
        };


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
            List<ContextMenuItem> contextMenuItems = [
                new ContextMenuItem() { Text = "Edit", Value = 1, Icon = "edit" }
            ];

            selectedSites = new List<Divelocation> { args.Data };
            contextMenuService.Open(args,
                contextMenuItems, 
                (e) => handleContexteMenuClick(e,args)
                //{
                //    switch (e.Value)
                //    {
                //        case 1:
                //            showEditDialog(selectedSites.FirstOrDefault());
                //            break;
                //        default:

                //            Console.WriteLine($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, SiteName: {args.Data.Name}");
                //            break;
                //    }
                //}
             );

        }

        private void handleContexteMenuClick(MenuItemEventArgs e, DataGridCellMouseEventArgs<Divelocation> args)
        {
            switch (e.Value)
            {
                case 1:
                    showEditDialog(selectedSites.FirstOrDefault());
                    break;
                default:
                    Console.WriteLine($"Menu item with Value={e.Value} clicked. Column: {args.Column.Property}, SiteName: {args.Data.Name}");
                    break;
            }
        }

        void onRowDblCLick(DataGridRowMouseEventArgs<Divelocation> args)
        {
            var site = args.Data;
            showEditDialog(site);
        }

        private void showEditDialog(Divelocation site)
        {
            var dialogParameters = new Dictionary<string, object>
            {
                { "Divesite", site },
                { "UpdateDivesiteCallback", EventCallback.Factory.Create<Divelocation>(this, updateSite)},
                { "DeleteDivesiteCallback", EventCallback.Factory.Create<Divelocation>(this, deleteSite)}
            };

            dialogService.Open<DivesiteDetailsComponent>("Edit Divesite", dialogParameters, editDialogOptions);

        }

        private async void updateSite(Divelocation site)
        {
            try
            {
                await siteService.UpdateAsync(site);
                dialogService.Close();
                notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = $"Site '{site.Name}' has been updated"
                }
                );
                await getSitesByTypeFilter();

            }
            catch (Exception ex)
            {

                await dialogService.Alert($"An error occurred: {ex.Message}", "Update error!", new AlertOptions() { OkButtonText = "OK" });
            }

        }

        private async void deleteSite(Divelocation site)
        {
            var siteName = site.Name;
            var deleteSite = await dialogService.Confirm($"Are you sure you want to delete '{siteName}'?", "Delete Site", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (deleteSite.GetValueOrDefault())
            {
                await siteService.DeleteAsync(site);

                notificationService.Notify(new NotificationMessage() { 
                    Severity = NotificationSeverity.Success, 
                    Summary = "Success", 
                    Detail = $"Site '{siteName}' has been deleted" }
                );

                await getSitesByTypeFilter();
            }

        }
        private void closeDialog()
        {
            dialogService.Close();
        }
    }
}
