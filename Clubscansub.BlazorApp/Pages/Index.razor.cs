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
using System.Drawing;

namespace Clubscansub.BlazorApp.Pages
{
    public partial class Index
    {
        //[Inject]
        //protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected DialogService Dialogservice { get; set; }

        [Inject]
        protected TooltipService Tooltipservice { get; set; }

        //[Inject]
        //protected ContextMenuService ContextMenuservice { get; set; }

        //[Inject]
        //protected NotificationService Notificationservice { get; set; }

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
            { EventTypeEnum.Stranddyk, false },
            { EventTypeEnum.Klubture, true },
            { EventTypeEnum.Other, true },
            { EventTypeEnum.Liveaboard, false },
            { EventTypeEnum.Rejse, false },
            { EventTypeEnum.NotAnEvent, false }
        };

        private Dictionary<CourseTypeEnum, bool> courseTypeFilters = new Dictionary<CourseTypeEnum, bool>() {
            {    CourseTypeEnum.BaseCourse, true },
            {    CourseTypeEnum.SpecialtyCourse, true },
            {    CourseTypeEnum.TechCourse, false },
            {    CourseTypeEnum.ProCourse, false }
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
            string bgClass = IconsTextsAndColors.ColorClasses.BG_DEFAULT;
            string fgClass = IconsTextsAndColors.ColorClasses.FG_DEFAULT;
            if (args.Data is Event)
            {
                switch ((args.Data as Event).EventType)
                {
                    case ClubScansub.Utility.EventTypeEnum.Bådtur:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_BOAT;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_BOAT;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.Stranddyk:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_BEACH;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_BEACH;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.Klubture:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_CLUB;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_CLUB;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.Other:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_OTHER;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_OTHER;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.Rejse:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_TRAVEL;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_TRAVEL;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.Liveaboard:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_LIVEABOARD;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_LIVEABOARD;
                        break;

                    case ClubScansub.Utility.EventTypeEnum.NotAnEvent:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_NOTEVENT;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_NOTEVENT;
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
                        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_BASE;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_BASE;
                        break;
                    case CourseTypeEnum.SpecialtyCourse:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_SPEC;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_SPEC;
                        break;
                    case CourseTypeEnum.TechCourse:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_TECH;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_TECH;
                        break;
                    case CourseTypeEnum.ProCourse:
                        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_PRO;
                        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_PRO;
                        break;
                    default:
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

            Tooltipservice.Open(args.Element, getToolTipText(args.Data), new TooltipOptions() { Delay=500, Duration=5000 });

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
                args.Attributes["class"] = IconsTextsAndColors.ColorClasses.SCHEDULER_SLOT_BG_WEEKEND;
            }

            if (!string.IsNullOrEmpty(holiday))
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(220,220,220,1));";
                args.Attributes["class"] = "holiday " + IconsTextsAndColors.ColorClasses.SCHEDULER_SLOT_BG_HOLIDAY;
                if (args.View.Text == IconsTextsAndColors.Texts.SCHEDULER_MONTH)
                    args.Attributes["after-text"] = holiday;
            }

            // Highlight today in month view
            if ((args.View.Text != IconsTextsAndColors.Texts.SCHEDULER_WEEK && args.View.Text != IconsTextsAndColors.Texts.SCHEDULER_DAY) && args.Start.Date == DateTime.Today)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
                args.Attributes["class"] = IconsTextsAndColors.ColorClasses.SCHEDULER_SLOT_BG_TODAY;

            }

            // Highlight working hours (9-18)
            if ((args.View.Text == IconsTextsAndColors.Texts.SCHEDULER_WEEK || args.View.Text == IconsTextsAndColors.Texts.SCHEDULER_DAY) && args.Start.Hour > 8 && args.Start.Hour < 19)
            {
                //args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
                args.Attributes["class"] = IconsTextsAndColors.ColorClasses.SCHEDULER_SLOT_BG_WORKHOURS;
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
                        return IconsTextsAndColors.Icons.EVENTTYPE_BASE_COURSE;
                    case CourseTypeEnum.SpecialtyCourse:
                        return IconsTextsAndColors.Icons.EVENTTYPE_SPEC_COURSE;
                    case CourseTypeEnum.TechCourse:
                        return IconsTextsAndColors.Icons.EVENTTYPE_TECH_COURSE;
                    case CourseTypeEnum.ProCourse:
                        return IconsTextsAndColors.Icons.EVENTTYPE_PRO_COURSE;

                }
            }

            switch ((item as Event).EventType)
            {
                case ClubScansub.Utility.EventTypeEnum.Bådtur:

                    return IconsTextsAndColors.Icons.EVENTTYPE_BOAT;

                case ClubScansub.Utility.EventTypeEnum.Stranddyk:
                    return IconsTextsAndColors.Icons.EVENTTYPE_BEACH;

                case ClubScansub.Utility.EventTypeEnum.Klubture:
                    return IconsTextsAndColors.Icons.EVENTTYPE_CLUB;

                case ClubScansub.Utility.EventTypeEnum.Other:
                    return IconsTextsAndColors.Icons.EVENTTYPE_OTHER;

                case ClubScansub.Utility.EventTypeEnum.Rejse:
                    return IconsTextsAndColors.Icons.EVENTTYPE_TRAVEL;

                case ClubScansub.Utility.EventTypeEnum.Liveaboard:
                    return IconsTextsAndColors.Icons.EVENTTYPE_LIVEABOARD;

                case ClubScansub.Utility.EventTypeEnum.NotAnEvent:
                    return IconsTextsAndColors.Icons.EVENTTYPE_NOT_EVENT;

                default:
                    return IconsTextsAndColors.Icons.EVENTTYPE_NOT_EVENT;
            }
        }
        private string getCourseSessionIcon(ICalendarEvent item)
        {
            if (item is CourseSession)
            {
                switch ((item as CourseSession).Sessiontype)
                {
                    case CourseSessionTypeEnum.AcademicSession:
                        return IconsTextsAndColors.Icons.COURSE_SESSION_ACADEMIC;
                    case CourseSessionTypeEnum.PoolSession:
                        return IconsTextsAndColors.Icons.COURSE_SESSION_POOL;
                    case CourseSessionTypeEnum.OpenWaterSession:
                        return IconsTextsAndColors.Icons.COURSE_SESSION_OW;
                }
            }
            return "";
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
