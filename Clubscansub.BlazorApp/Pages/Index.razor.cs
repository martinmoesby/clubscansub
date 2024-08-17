using System.Net.Http;
using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Microsoft.Win32;
using Radzen;
using Radzen.Blazor;

namespace Clubscansub.BlazorApp.Pages
{
    public partial class Index
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

        [Inject]
        protected EventService  eventService { get; set; }

        private IList<Event> events = new List<Event>();
        private IList<Course> courses = new List<Course>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                events = await eventService.GetAllAsync();
                StateHasChanged();
            }
        }

        private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Event> args)
        {
            string baseClassString = "rz-px-1";
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
    }
}