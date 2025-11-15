using ClubScansub.Models;
using ClubScansub.Models.Interface;
using ClubScansub.Utility.Enums;
using ClubScansub.Utility.Extensions;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ClubScansub.Blazor.UIComponents.Common
{
    public partial class SeatComponent : ComponentBase
    {
        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public bool ShowText { get; set; } = true;

        [Parameter]
        public string Text { get; set; } = "Status";

        [Parameter]
        public IconSizeEnum Size { get; set; } = IconSizeEnum.Medium;

        private string sizeStyle => Size.ToFontSizeStyle();

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

        }
    }
}
