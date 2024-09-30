using ClubScansub.Service;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace ClubScansub.BlazorApp.Pages.Account
{
    public partial class ConfirmEmail : ComponentBase
    {
        [Inject]
        NavigationManager navigationManager { get; set; }

        [Inject]
        UserService userService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await  base.OnInitializedAsync();

            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userid", out var useridParsed))
                userid = new Guid(useridParsed.First());
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var codeParsed))
                code = codeParsed.First();

            var confirmationResult = await userService.EmailConfirmAsync(userid, code);

            confirmationStatus = confirmationResult.Succeeded ? "Thank you for confirming your email." : $"Error confirming your email: {confirmationResult.Errors.First().Description}.";
        }

        private string confirmationStatus = "Validating your email...";
        private Guid userid = Guid.Empty;
        private string code = string.Empty;

    }
}
