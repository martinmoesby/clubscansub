using ClubScansub.Extensions;
using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Events
{
    public partial class EventComponent : ComponentBase
    {
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }

        [Inject]
        protected EventService eventService { get; set; }

        [Inject]
        protected EventUserService eventUserService { get; set; }
        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        MemberService memberService { get; set; }

        [Inject]
        NotificationService notificationService { get; set; }

        [Parameter]
        public int EventId { get; set; }
        [Parameter]
        public EventCallback<Event> EventStatusChanged { get; set; }

        private ApplicationUser currentUser { get; set; } = new();
        Event Event { get; set; } = new Event();
        IList<EventUser> enrolledUsers = new List<EventUser>();

        private bool isLoading = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                if (authState.User != null && authState.User.Identity != null)
                {
                    if ( authState.User.Identity.IsAuthenticated)
                    {
                        var user = await memberService.GetAsync(authState.User.GetIdentityId());
                        currentUser = user;
                    }
                }
                var item = await eventService.GetAsync(EventId.ToString());
                if (item != null) { 
                    Event = item;
                }
                enrolledUsers = await eventUserService.GetUsersByEvent(EventId);
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task OnLoadEnrolledUsers()
        {
            enrolledUsers = await eventUserService.GetUsersByEvent(EventId);
            Event.Participants = enrolledUsers;
            StateHasChanged();
        }

        private async Task enrollExistingUser(string selectedUser)
        {
            if (string.IsNullOrEmpty(selectedUser))
            {
                return;
            }

            if (selectedUser == "scansub")
            {
                Event.FixedParticipants++;
                await eventService.UpdateAsync(Event);
                StateHasChanged();
                return;
            }

            if (Event.Participants.Any(x => x.ApplicationUserId == selectedUser))
            {
                notificationService.Notify(NotificationSeverity.Warning, "This user is already signed up for this event");

            }
            else
            {
                await eventUserService.AddUserToEvent(selectedUser, EventId);
                await OnLoadEnrolledUsers();
                notificationService.Notify(NotificationSeverity.Success, $"User added to this for the event");
            }
        }

        private async Task releaseReservedSpot()
        {
            Event.FixedParticipants--;
            if (Event.FixedParticipants < 0)
                Event.FixedParticipants = 0;

            await eventService.UpdateAsync(Event);
            StateHasChanged();
            return;
        }
        private async Task removeUserFromEvent(ApplicationUser user)
        {
            var confirm = (await dialogService.Confirm($"Do you want to remove {user.Name} from this trip?", "Remove user"));
            if (confirm.Value)
            {
                //TODO: Remove user from course and return any deposits made for the enrollment
                await eventUserService.RemoveUserFromEvent(user.Id, EventId);
                await OnLoadEnrolledUsers();
            }
        }

        private async Task cancelEvent()
        {
            var doCancel = await dialogService.Confirm("Do you want to cancel this event? Participants will be refunded 100%", "Cancel Event?", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No"
            });
            if (doCancel.GetValueOrDefault())
            {
                Event.IsCancelled = true;
                await eventService.CancelEventAsync(Event);
                await EventStatusChanged.InvokeAsync(Event);
            }
        }

        private async Task reactivateEvent()
        {
            var doCancel = await dialogService.Confirm("Do you want to activate this event?", "Activate Event?", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No"
            });
            if (doCancel.GetValueOrDefault())
            {
                Event.IsCancelled = false;
                await eventService.ActivateEventAsync(Event);
                await EventStatusChanged.InvokeAsync(Event);
            }
        }

    }
}
