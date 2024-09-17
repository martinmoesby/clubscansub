using Microsoft.AspNetCore.Components;

namespace Clubscansub.BlazorApp.Components.Common
{
    public partial class SearchComponent : ComponentBase
    {
        [Parameter]
        public string Placeholder { get; set; }
        [Parameter]
        public EventCallback<string> Search { get; set; }

        [Parameter]
        public EventCallback Reset { get; set; }

        private string searchFilter;

        async void onSearchFilterChanged()
        {
            await Search.InvokeAsync(searchFilter);
        }

        async void resetSearchFilter()
        {
            searchFilter = null;
            await Reset.InvokeAsync();
        }
    }
}
