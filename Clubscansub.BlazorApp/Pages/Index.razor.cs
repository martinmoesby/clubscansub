using ClubScansub.Blazor.UIComponents.Events;
using ClubScansub.Blazor.UIComponents.Courses;
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

namespace ClubScansub.BlazorApp.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        protected DialogService Dialogservice { get; set; }

        [Inject]
        protected TooltipService Tooltipservice { get; set; }

        [Inject]
        protected EventService eventService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private Guid linkId { get; set; }

        private RadzenScheduler<ICalendarEvent> scheduler { get; set; }

        private IList<Event> events = new List<Event>();
        private IList<CourseSession> sessions = new List<CourseSession>();

        private IList<ICalendarEvent> filteredData = new List<ICalendarEvent>();

        private List<object> holidays = new();

        private Dictionary<EventTypeEnum, bool> eventTypeFilters = new Dictionary<EventTypeEnum, bool>() 
        {
            { EventTypeEnum.Bådtur, true },
            { EventTypeEnum.Stranddyk, true },
            { EventTypeEnum.Klubture, true },
            { EventTypeEnum.Rejse, true },
            { EventTypeEnum.Liveaboard, false },
            { EventTypeEnum.Other, false },
            //{ EventTypeEnum.NotAnEvent, false }
        };

        private Dictionary<CourseTypeEnum, bool> courseTypeFilters = new Dictionary<CourseTypeEnum, bool>() {
            {    CourseTypeEnum.BaseCourse, true },
            {    CourseTypeEnum.SpecialtyCourse, true },
            {    CourseTypeEnum.TechCourse, false },
            {    CourseTypeEnum.ProCourse, false }
        };

        private bool showAllEvents { get; set; } = false;
        private bool showAllCourses { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("eventId", out var param))
            {
                linkId = new Guid(param);
                if (linkId != Guid.Empty)
                    await gotoEvent();
            }
        }

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

                if ((args.Data as Event).IsCancelled)
                {
                    bgClass = "rz-background-color-secondary-lighter";
                    fgClass = "rz-color-base-500";
                    args.Attributes["style"] = "text-decoration:line-through;cursor:not-allowed;";
                }
            }

            if (args.Data is CourseSession)
            {
                (bgClass, fgClass) = IconsTextsAndColors.GetCourseClasses((args.Data as CourseSession).Course?.CourseType);
                //switch ((args.Data as CourseSession).Course?.CourseType)
                //{
                //    case CourseTypeEnum.BaseCourse:
                //        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_BASE;
                //        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_BASE;
                //        break;
                //    case CourseTypeEnum.SpecialtyCourse:
                //        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_SPEC;
                //        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_SPEC;
                //        break;
                //    case CourseTypeEnum.TechCourse:
                //        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_TECH;
                //        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_TECH;
                //        break;
                //    case CourseTypeEnum.ProCourse:
                //        bgClass = IconsTextsAndColors.ColorClasses.BG_COURSE_PRO;
                //        fgClass = IconsTextsAndColors.ColorClasses.FG_COURSE_PRO;
                //        break;
                //    default:
                //        break;

                //}
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
                    { "EventId", args.Data.Id },
                    { "EventStatusChanged", EventCallback.Factory.Create<Event>(this, updateEventStatus) }
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
            if ((args.Data is Event && (args.Data as Event).IsCancelled) || (args.Data is CourseSession && (args.Data as CourseSession).Course.IsCancelled))
                return;

            Tooltipservice.Open(
                args.Element,
                getToolTipText(args.Data),
                new TooltipOptions { Delay = 500, Duration = 5000, Position = TooltipPosition.Top }
            );

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

        private RenderFragment<TooltipService> getToolTipText(ICalendarEvent item)
        {
            RenderFragment fragment(TooltipService context) => builder =>
            {
                builder.OpenElement(0, "div");
                if (item is CourseSession)
                {
                    builder.AddContent(1, "This is a course");
                }
                else
                {
                    builder.AddMarkupContent(1, $@"
                        <div style='min-width:100px;max-width:250px;display:flex;flex-direction:column' >
                            <h5 style='white-space: nowrap; overflow: hidden; text-overflow: ellipsis'>{item.Title}</h5>
                            <div style='white-space: wrap;'>{item.Divelocation?.Description}</div>
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
                        ");
                }
                builder.CloseElement();
            };
            return fragment;

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
        private void setAllCourseFilter()
        {
            showAllCourses = !showAllCourses;
            foreach (var item in courseTypeFilters)
            {
                courseTypeFilters[item.Key] = showAllCourses;
            }
            applyFilter();
        }

        private void setCourseFilters(Dictionary<CourseTypeEnum, bool> filters)
        {
            courseTypeFilters = filters;
            showAllCourses = !filters.Any(x => x.Value == false);
            applyFilter();
        }
        private void setEventFilters(Dictionary<EventTypeEnum, bool> filters)
        {
            eventTypeFilters = filters;
            showAllEvents = !filters.Any(x => x.Value == false);
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

     
        private async Task gotoEvent()
        {
            var selectedEvent = await eventService.GetAsync(linkId);
            scheduler.CurrentDate = selectedEvent.StartDateAndTime;
            await scheduler.Reload();
            await Dialogservice.OpenAsync<EventComponent>(selectedEvent.Title, new Dictionary<string, object>()
                {
                    { "EventId", selectedEvent.Id },
                    { "EventStatusChanged", EventCallback.Factory.Create<Event>(this, updateEventStatus) }
                }, new DialogOptions()
                {
                    Width = "800px",
                    Height = "80%",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });

        }
        
        private void updateEventStatus(Event item)
        {
            var e = events.FirstOrDefault(x => x.Id == item.Id);
            if (e != null)
            {
                e.IsCancelled = item.IsCancelled;
                applyFilter();
                StateHasChanged();
            }

        }
    }
}
