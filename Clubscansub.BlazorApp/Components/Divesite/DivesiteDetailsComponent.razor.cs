using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;

namespace Clubscansub.BlazorApp.Components.Divesite
{
    public partial class DivesiteDetailsComponent : ComponentBase
    {
        [Inject]
        DialogService dialogService { get; set; }
        [Inject]
        SiteService siteService { get; set; }

        [Inject]
        IConfiguration configuration { get; set; }

        [Parameter]
        public EventCallback<Divelocation> UpdateDivesiteCallback { get; set; }
        [Parameter]
        public EventCallback<Divelocation> DeleteDivesiteCallback { get; set; }
        [Parameter]
        public Divelocation Divesite { get; set; } = new();

        private string googleMapsAPIKey = "";
        private string googleMapsMapId = "";
        private GoogleMapPosition divesitePosition;
        private GoogleMapPosition meetinLocationPosition;
        private RadzenGoogleMap meetingMap;
        private RadzenGoogleMap locationMap;
        private Dictionary<string, object> googleMapOptions = new Dictionary<string, object> { { "disableDoubleClickZoom", true } };

    protected override void OnInitialized()
        {
            base.OnInitialized();
            googleMapsAPIKey = configuration["GoogleMap:ApiKey"];
            googleMapsMapId = configuration["GoogleMap:MapId"];
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (divesitePosition == null && Divesite != null)
            {
                divesitePosition = new GoogleMapPosition() { Lat = Divesite.Latitude, Lng = Divesite.Longitude };
                if (Divesite.MeetingLocation != null)
                {
                    meetinLocationPosition = new GoogleMapPosition { Lat = Double.Parse(Divesite.MeetingLocation.Latitude, System.Globalization.CultureInfo.InvariantCulture), Lng = Double.Parse(Divesite.MeetingLocation.Longitude, System.Globalization.CultureInfo.InvariantCulture) };
                }
                StateHasChanged();
            }
        }


        private void onInvalidSubmit()
        {
            dialogService.Alert("Invalid information - please check all data and try again", "Invalid", new AlertOptions() { OkButtonText = "OK" });
        }

        private void deleteDivesiteClick()
        {
            DeleteDivesiteCallback.InvokeAsync(Divesite);
        }

        private void onMapClick(GoogleMapClickEventArgs args)
        {
            Console.WriteLine($"Map clicked at Lat: {args.Position.Lat}, Lng: {args.Position.Lng}");
        }

        private void onMarkerClick(RadzenGoogleMapMarker marker)
        {

        }
    }
}
