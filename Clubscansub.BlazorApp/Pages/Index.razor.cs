using Clubscansub.BlazorApp.Components.Events;
using Clubscansub.BlazorApp.Components.Courses;
using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Service;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nager.Date;
using Nager.Date.Model;
using Radzen;
using Radzen.Blazor;
using System.Diagnostics;
using System.Globalization;

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

        private RadzenScheduler<ICalendarEvent> scheduler { get; set; }

        private IList<Event> events = new List<Event>();
        private IList<CourseSession> sessions = new List<CourseSession>();

        private IList<ICalendarEvent> filteredData = new List<ICalendarEvent>();
        //private IList<Course> courses = new List<Course>();
        private List<object> holidays = new();

        private Dictionary<EventTypeEnum, bool> eventTypeFilters = new Dictionary<EventTypeEnum, bool>() 
        {
            { EventTypeEnum.Bådtur, true },
            { EventTypeEnum.Stranddyk, true },
            { EventTypeEnum.Klubture, true },
            { EventTypeEnum.Other, true },
            { EventTypeEnum.Liveaboard, true },
            { EventTypeEnum.Rejse, true },
            { EventTypeEnum.NotAnEvent, false }
        };

        private Dictionary<CourseTypeEnum, bool> courseTypeFilters = new Dictionary<CourseTypeEnum, bool>() {
            {    CourseTypeEnum.BaseCourse, true },
            {    CourseTypeEnum.SpecialtyCourse, true },
            {    CourseTypeEnum.TechCourse, true },
            {    CourseTypeEnum.ProCourse, true }
        };

        private bool showAllEvents { get; set; } = false;
        private bool showAllCourses { get; set; } = false;

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

        private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<ICalendarEvent> args)
        {
            string baseClassString = "rz-pl-3 rz-event-content";
            string bgClass = "rz-background-color-primary";
            string fgClass = "rz-color-white";
            if (args.Data is Event)
            {
                switch ((args.Data as Event).EventType)
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
                        bgClass = "rz-background-color-info";

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

            }
            if (args.Data is CourseSession)
            {
                switch ((args.Data as CourseSession).Course?.CourseType)
                {
                    case CourseTypeEnum.BaseCourse:
                        bgClass = "rz-background-color-danger-lighter";
                        fgClass = "rz-color-black";
                        break;
                    case CourseTypeEnum.SpecialtyCourse:
                        bgClass = "rz-background-color-danger";
                        break;
                    case CourseTypeEnum.TechCourse:
                        bgClass = "rz-background-color-danger-dark";
                        break;
                    case CourseTypeEnum.ProCourse:
                        bgClass = "rz-background-color-danger-darker";
                        break;
                    default:
                        bgClass = "rz-background-color-info-lighter";
                        break;

                }
            }
            args.Attributes["class"] = $"{baseClassString} {bgClass} {fgClass}";
        }

        private async void OnAppointmentClick(SchedulerAppointmentSelectEventArgs<ICalendarEvent> args)
        {
            var sideDialogOptions = new SideDialogOptions()
            {
                Width = "80%",
                CloseDialogOnOverlayClick = true,
                ShowClose = true,
                Position = DialogPosition.Right,
                ShowMask = true

            };
            var dialogOptions = new DialogOptions()
            {
                Width = "800px",
                Height="80%",
                CloseDialogOnOverlayClick = true,
                ShowClose = true,
            };

            Tooltipservice.Close();
            if (args.Data is Event)
            {
                await Dialogservice.OpenAsync<EventComponent>(args.Data.Title, new Dictionary<string, object>()
                {
                    {
                        "EventId",args.Data.Id
                    }
                }, 
                dialogOptions);
            }
            if (args.Data is CourseSession)
            {
                await Dialogservice.OpenAsync<CourseEventComponent>(args.Data.Title, new Dictionary<string, object>()
                {
                    { "Id",((args.Data as CourseSession).Course?.Id)},
                    { "SessionId",args.Data.Id }
                }, 
                dialogOptions);
            }
        }
        private void OnMouseOverAppointment(SchedulerAppointmentMouseEventArgs<ICalendarEvent> args)
        {
            if (args.Data is Event)
            {
                Tooltipservice.Open(args.Element, getToolTipText(args.Data), new TooltipOptions() { Delay=500, Duration=5000 });
            }

        }
        private void OnMouseLeaveAppointment(SchedulerAppointmentMouseEventArgs<ICalendarEvent> args)
        {
            Tooltipservice.Close();
        }
        private void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            string? holiday = getHolidayName(args.Start.Date);

            if (args.Start.DayOfWeek == DayOfWeek.Sunday || args.Start.DayOfWeek == DayOfWeek.Saturday)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(220,220,220,1));";
                args.Attributes["class"] = "rz-background-color-base-lighter";
            }

            if (!string.IsNullOrEmpty(holiday))
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(220,220,220,1));";
                args.Attributes["class"] = "holiday rz-background-color-danger-lighter";
                if (args.View.Text == "Måned")
                    args.Attributes["after-text"] = holiday;
            }

            // Highlight today in month view
            if ((args.View.Text != "Uge" && args.View.Text != "Dag") && args.Start.Date == DateTime.Today)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
                args.Attributes["class"] = "rz-background-color-warning-lighter";

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


            events = (await eventService.GetAllByDateAsync(startDate, endDate)).ToList<Event>();
            sessions = (await eventService.GetAllCourseSessionsByDateAsync(startDate, endDate)).ToList<CourseSession>();
            applyFilter();

            StateHasChanged();
        }

        private string getAppointmentIcon(ICalendarEvent item)
        {
            if (item is CourseSession)
            {
                switch ((item as CourseSession).Course.CourseType)
                {
                    case CourseTypeEnum.BaseCourse:
                        return "school";
                    case CourseTypeEnum.SpecialtyCourse:
                        return "military_tech";
                    case CourseTypeEnum.TechCourse:
                        return "editor_choice";
                    case CourseTypeEnum.ProCourse:
                        return "social_leaderboard";

                }
            }

            switch ((item as Event).EventType)
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

        private string getToolTipText(ICalendarEvent item)
        {

            if (item is Course)
            {
                return "This is a course";
            }

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
                        Price: {item.Price:C2}
                    </div>
                    <div>
                        Free seats: {item.FreeSpots:N0}
                    </div>
                </div>
            </div>
            ";

        }

        private bool getEventFilter(EventTypeEnum type)
        {
            return eventTypeFilters[type];
        }
        private async void setEventFilter(EventTypeEnum type)
        {
            eventTypeFilters[type] = !eventTypeFilters[type];
            var allToggled = !eventTypeFilters.Any(x => x.Value == false);
            showAllEvents = allToggled;
            applyFilter();
        }
        private void setAllEventFilter()
        {
            showAllEvents = !showAllEvents;
            foreach (var item in eventTypeFilters)
            {
                eventTypeFilters[item.Key] = showAllEvents;
            }
            applyFilter();
        }

        private bool getCourseFilter(CourseTypeEnum type)
        {
            return courseTypeFilters[type];
        }
        private async void setCourseFilter(CourseTypeEnum type)
        {
            courseTypeFilters[type] = !courseTypeFilters[type];
            var allToggled = !courseTypeFilters.Any(x => x.Value == false);
            showAllCourses = allToggled;
            applyFilter();
        }
        private void setAllCourseFilter()
        {
            showAllCourses = !showAllCourses;
            foreach (var item in courseTypeFilters)
            {
                courseTypeFilters[item.Key] = showAllCourses;
            }
            applyFilter();
        }

        private void applyFilter()
        {
            var selectedEventTypes = eventTypeFilters.Where(x => x.Value == true).Select(x => x.Key);
            var selectedCourseType = courseTypeFilters.Where(x => x.Value == true).Select(x => x.Key);

            var filteredCoursesessions = sessions.Where(x => selectedCourseType.Contains(x.Course.CourseType)).ToList<ICalendarEvent>();
            var filteredEvents = events.Where(x => selectedEventTypes.Contains(x.EventType)).ToList<ICalendarEvent>();

            filteredData = [.. filteredCoursesessions, .. filteredEvents];
            


        }
    }
}
