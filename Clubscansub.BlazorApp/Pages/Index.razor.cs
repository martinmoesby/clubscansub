using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http;
using Clubscansub.BlazorApp.Components.Scheduler;
using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.JSInterop;
using Microsoft.Win32;
using Nager.Date;
using Nager.Date.Model;
using Radzen;
using Radzen.Blazor;

namespace Clubscansub.BlazorApp.Pages
{
    public partial class Index
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager Navigationmanager { get; set; }

        [Inject]
        protected DialogService Dialogservice { get; set; }

        [Inject]
        protected TooltipService Tooltipservice { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuservice { get; set; }

        [Inject]
        protected NotificationService Notificationservice { get; set; }

        [Inject]
        protected EventService eventService { get; set; }

        private RadzenScheduler<Event> scheduler { get; set; }

        private IList<Event> events = new List<Event>();
        private IList<Course> courses = new List<Course>();
        private List<object> holidays = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                //events = await eventService.GetAllAsync();
                scheduler.DefaultCulture = CultureInfo.CurrentUICulture;
                StateHasChanged();
            }
        }

        private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Event> args)
        {
            string baseClassString = "rz-pl-3 rz-event-content";
            string bgClass = "rz-background-color-primary";
            string fgClass = "rz-color-white";

            switch (args.Data.EventType)
            {
                case ClubScansub.Utility.EventTypeEnum.Bådtur:
                    bgClass = "rz-background-color-primary-darker";

                    break;

                case ClubScansub.Utility.EventTypeEnum.Stranddyk:
                    bgClass = "rz-background-color-primary-lighter";
                    fgClass = "rz-color-black";

                    break;

                case ClubScansub.Utility.EventTypeEnum.Klubture:
                    bgClass = "rz-background-color-primary";

                    break;


                case ClubScansub.Utility.EventTypeEnum.Other:
                    bgClass = "rz-background-color-info-darker";

                    break;


                case ClubScansub.Utility.EventTypeEnum.Rejse:
                    bgClass = "rz-background-color-success";

                    break;

                case ClubScansub.Utility.EventTypeEnum.Liveaboard:
                    bgClass = "rz-background-color-success-darker";

                    break;


                case ClubScansub.Utility.EventTypeEnum.NotAnEvent:
                    bgClass = "rz-background-color-danger-light";

                    break;
                default:
                    break;
            }
            args.Attributes["class"] = $"{baseClassString} {bgClass} {fgClass}";
        }

        private async void OnAppointmentClick(SchedulerAppointmentSelectEventArgs<Event> args)
        {
            Tooltipservice.Close();
            await Dialogservice.OpenAsync<EventComponent>(args.Data.Title, new Dictionary<string, object>()
            {
                {
                "EventId",args.Data.Id
                }
            }, new DialogOptions()
            {
                Width="800px",
                Height="600px",
                CloseDialogOnEsc=false,
                CloseDialogOnOverlayClick=true,
                ShowClose=true,
                Resizable=true,
                Draggable=true
            });
        }
        private void OnMouseOverAppointment(SchedulerAppointmentMouseEventArgs<Event> args)
        {
            if (args.Data is Event)
            {
                Tooltipservice.Open(args.Element, getToolTipText(args.Data), new TooltipOptions() { Delay=500, Duration=5000 });
            }

        }
        private void OnMouseLeaveAppointment(SchedulerAppointmentMouseEventArgs<Event> args)
        {
            Tooltipservice.Close();
        }
        private void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            string? holiday = getHolidayName(args.Start.Date);

            if (!string.IsNullOrEmpty(holiday))
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(220,220,220,1));";
                args.Attributes["class"] = "holiday rz-background-color-danger-lighter";
                if (args.View.Text == "Måned")
                    args.Attributes["after-text"] = holiday;
            }

            // Highlight today in month view
            if (args.View.Text == "Måned" && args.Start.Date == DateTime.Today)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
                args.Attributes["class"] = "holiday rz-background-color-warning-lighter";
                args.Attributes["after-text"] = "Today";
            }

            // Highlight working hours (9-18)
            if ((args.View.Text == "Uge" || args.View.Text == "Dag") && args.Start.Hour > 8 && args.Start.Hour < 19)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
                args.Attributes["class"] = "rz-background-color-success-lighter";
            }
        }

        private async void OnLoadData()
        {
            var startDate = scheduler.SelectedView.StartDate;
            var endDate = scheduler.SelectedView.EndDate;
            events = (await eventService.GetAllByDateAsync(startDate, endDate)).ToList();
            StateHasChanged();
        }

        private string getAppointmentIcon(Event @event)
        {
            switch (@event.EventType)
            {
                case ClubScansub.Utility.EventTypeEnum.Bådtur:

                    return "sailing";

                case ClubScansub.Utility.EventTypeEnum.Stranddyk:
                    return "beach_access";

                case ClubScansub.Utility.EventTypeEnum.Klubture:
                    return "groups";

                case ClubScansub.Utility.EventTypeEnum.Other:
                    return "other_admission";

                case ClubScansub.Utility.EventTypeEnum.Rejse:
                    return "flight_takeoff";

                case ClubScansub.Utility.EventTypeEnum.Liveaboard:
                    return "houseboat";

                case ClubScansub.Utility.EventTypeEnum.NotAnEvent:
                    return "unknown_document";

                default:
                    return "";
            }
        }

        private string? getHolidayName(DateTime date)
        {
            if (scheduler.DefaultCulture != null)
            {

                var year = date.Year;
                var culture = scheduler.DefaultCulture;

                var region = new RegionInfo(culture.LCID);
                CountryCode countryCode = (CountryCode)Enum.Parse(typeof(CountryCode), region.Name);
                PublicHoliday[] holidays;
                var IsHoliday = DateSystem.IsPublicHoliday(date, countryCode, out holidays);
                if (holidays.Length != 0)
                {
                    return holidays.FirstOrDefault(x => x.Date == date)?.LocalName;
                }
                return null;
            }
            return null;

        }

        private string getToolTipText(Event item)
        {

            return $@"
<div style='min-width:250px;max-width:400px;display:flex;flex-direction:column' >
    <h5 style='white-space: nowrap; overflow: hidden; text-overflow: ellipsis'>{item.Title}</h5>
    <div style='white-space: nowrap; overflow: hidden; text-overflow: ellipsis'>{item.Divelocation?.Description}</div>
    <div style='display:flex;justify-content:space-between'>
        <div>
            Start: {item.StartDateAndTime.ToShortTimeString()}
        </div>
        <div>
            End: {item.EndDateAndTime.ToShortTimeString()}
        </div>
    </div>
    <div style='display:flex;justify-content:space-between'>
        <div>
            Price: {item.PremiumPrice:C2}
        </div>
        <div>
            Free seats: {item.FreeSpots:N0}
        </div>
    </div>
</div>
";
        }
    }
}
