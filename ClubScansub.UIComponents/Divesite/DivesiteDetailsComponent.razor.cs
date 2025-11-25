using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;

namespace ClubScansub.Blazor.UIComponents.Divesite
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
        private RadzenGoogleMap map;
        List<RadzenGoogleMapMarker> markers = new();
        private GoogleMapPosition divesitePosition  = new GoogleMapPosition() { Lat = 0, Lng = 0 };
        private GoogleMapPosition meetinLocationPosition;
        private bool fitBoundsToMarkers = false;
        private Dictionary<string, object> googleMapOptions = new Dictionary<string, object> { { "disableDoubleClickZoom", true } };
        private List<Address> meetingLocations = new();
        private Address divesiteMeetingLocation = new();
        private List<Certificate> certificates = new();
        private DateTime? startTime;
        private DateTime? endTime;


        //string MeetingLabel => "<svg width=\"24\" height=\"32\" viewBox=\"0 0 24 32\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\n<path d=\"M10.1003 0.0756569C5.57812 0.807465 1.96912 3.85979 0.561802 8.14431C0.180262 9.3077 0.0426568 10.0895 0.011383 11.3718C-0.0136361 12.2599 -0.00112654 12.604 0.0864402 13.0918C0.655624 16.2818 3.30139 21.0416 7.79857 26.9962C9.38728 29.0915 11.7516 32 11.8767 32C12.0018 32 14.5912 28.8101 16.0736 26.8398C20.5333 20.8978 23.104 16.2505 23.6669 13.0918C23.817 12.2537 23.7732 10.5587 23.5731 9.57665C22.6286 4.8543 19.0134 1.19526 14.291 0.188242C13.628 0.0443823 13.3528 0.0193636 12.0956 0.00685407C11.051 -0.0119102 10.5069 0.00685445 10.1003 0.0756569ZM13.0964 6.16155C15.3793 6.63065 17.2308 8.48207 17.6999 10.7651C18.3691 13.9925 16.2925 17.1512 13.0651 17.8142C12.2895 17.9768 11.0886 17.9455 10.3443 17.7391C8.18011 17.1512 6.49757 15.3748 6.05349 13.2044C5.36546 9.86437 7.59216 6.64942 11.001 6.08649C11.4451 6.01143 12.571 6.05522 13.0964 6.16155Z\" fill=\"green\"/>\n</svg>\n";
        //string DivesiteLabel => "<svg width=\"24\" height=\"32\" viewBox=\"0 0 24 32\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\n<path d=\"M10.1003 0.0756569C5.57812 0.807465 1.96912 3.85979 0.561802 8.14431C0.180262 9.3077 0.0426568 10.0895 0.011383 11.3718C-0.0136361 12.2599 -0.00112654 12.604 0.0864402 13.0918C0.655624 16.2818 3.30139 21.0416 7.79857 26.9962C9.38728 29.0915 11.7516 32 11.8767 32C12.0018 32 14.5912 28.8101 16.0736 26.8398C20.5333 20.8978 23.104 16.2505 23.6669 13.0918C23.817 12.2537 23.7732 10.5587 23.5731 9.57665C22.6286 4.8543 19.0134 1.19526 14.291 0.188242C13.628 0.0443823 13.3528 0.0193636 12.0956 0.00685407C11.051 -0.0119102 10.5069 0.00685445 10.1003 0.0756569ZM13.0964 6.16155C15.3793 6.63065 17.2308 8.48207 17.6999 10.7651C18.3691 13.9925 16.2925 17.1512 13.0651 17.8142C12.2895 17.9768 11.0886 17.9455 10.3443 17.7391C8.18011 17.1512 6.49757 15.3748 6.05349 13.2044C5.36546 9.86437 7.59216 6.64942 11.001 6.08649C11.4451 6.01143 12.571 6.05522 13.0964 6.16155Z\" fill=\"red\"/>\n</svg>\n";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            googleMapsAPIKey = configuration["GoogleMap:ApiKey"];
            googleMapsMapId = configuration["GoogleMap:MapId"];
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {

                if (Divesite.Latitude != 0 && Divesite.Longitude != 0)
                {
                    divesitePosition = new GoogleMapPosition() { Lat = Divesite.Latitude, Lng = Divesite.Longitude };
                    // Add a marker for the divesite location
                    markers.Add(new RadzenGoogleMapMarker() { Position = divesitePosition, Title = Divesite.Name });
                }
                if (Divesite.MeetingLocation != null && !string.IsNullOrEmpty(Divesite.MeetingLocation.Latitude) && !string.IsNullOrEmpty(Divesite.MeetingLocation.Longitude))
                {
                    divesiteMeetingLocation = Divesite.MeetingLocation;
                    meetinLocationPosition = new GoogleMapPosition { Lat = Double.Parse(Divesite.MeetingLocation.Latitude, System.Globalization.CultureInfo.InvariantCulture), Lng = Double.Parse(Divesite.MeetingLocation.Longitude, System.Globalization.CultureInfo.InvariantCulture) };
                    // Add a marker for the meeting location
                    markers.Add(new RadzenGoogleMapMarker() { Position = meetinLocationPosition, Title = "Meeting Location" });
                }
                fitBoundsToMarkers = markers.Count > 1;

                meetingLocations = await siteService.GetMeetingLocations();
                certificates = await siteService.GetCertificatesForEvents();

                startTime =DateTime.Today.Add(Divesite.DefaultStartTime);
                endTime = DateTime.Today.Add(Divesite.DefaultStartTime + Divesite.DefaultDuration);

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

        private void onRequiredCertificateChanged(Certificate selectedCertificate)
        {
            Divesite.Certificate = selectedCertificate;
        }

        private void onMapClick(GoogleMapClickEventArgs args)
        {
            Console.WriteLine($"Map clicked at Lat: {args.Position.Lat}, Lng: {args.Position.Lng}");
        }

        private async void onMarkerClick(RadzenGoogleMapMarker marker)
        {
//            var message = $"Custom information about <b>{marker.Title}</b>";

//            var code = $@"
//var map = Radzen['{map.UniqueID}'].instance;
//var marker = map.markers.find(m => m.title == '{marker.Title}');
//if(window.infoWindow) {{window.infoWindow.close();}}
//window.infoWindow = new google.maps.InfoWindow({{content: '{message}'}});
//setTimeout(() => window.infoWindow.open(map, marker), 200);
//";

//            await JSRuntime.InvokeVoidAsync("eval", code);
        }

        private void onImageClick()
        {

        }
    }
}
