using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Service;
using ClubScansub.Utility;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Events
{
    public partial class EditEventComponent : ComponentBase
    {
        //[Inject]
        //protected EventService eventService { get; set; }

        //[Inject]
        //protected EventUserService eventUserService { get; set; }
        [Inject]
        DialogService dialogService { get; set; }


        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        IList<EventUser> enrolledUsers = new List<EventUser>();

        private bool isLoading = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            //    var item = await eventService.GetAsync(EventId.ToString());
            //    if (item != null) { 
            //        Event = item;
            //    }
            //    enrolledUsers = await eventUserService.GetUsersByEvent(EventId);
            isLoading = false;
            StateHasChanged();
            }
        }


        //private async Task cancelEvent()
        //{
        //    var doCancel = await dialogService.Confirm("Do you want to cancel this event? Participants will be refunded 100%", "Cancel Event?", new ConfirmOptions()
        //    {
        //        OkButtonText = "Yes",
        //        CancelButtonText = "No"
        //    });
        //    if (doCancel.GetValueOrDefault())
        //    {
        //        Event.IsCancelled = true;
        //        await eventService.CancelEventAsync(Event);
        //        await EventChanged.InvokeAsync(Event);
        //    }
        //}

        //private async Task reactivateEvent()
        //{
        //    var doCancel = await dialogService.Confirm("Do you want to activate this event?", "Activate Event?", new ConfirmOptions()
        //    {
        //        OkButtonText = "Yes",
        //        CancelButtonText = "No"
        //    });
        //    if (doCancel.GetValueOrDefault())
        //    {
        //        Event.IsCancelled = false;
        //        await eventService.ActivateEventAsync(Event);
        //        await EventChanged.InvokeAsync(Event);
        //    }
        //}

        private async Task cancelChanges()
        {
            dialogService.Close();
        }

        private async Task saveChanges()
        {
            var doSave = await dialogService.Confirm("Do you want to save the changes to this event?", "Save changes?", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No"
            });

            if (doSave.GetValueOrDefault())
            {
                await EventChanged.InvokeAsync(Event);
                dialogService.Close();
            }
        }
    }
}
