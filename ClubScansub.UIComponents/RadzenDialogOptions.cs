using Microsoft.CodeAnalysis.CSharp.Syntax;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Blazor.UIComponents.RadzenDialogOptions
{
    public class YesNoConfirmOptions : ConfirmOptions
    {
        public YesNoConfirmOptions()
        {
            OkButtonText = "Yes";
            CancelButtonText = "No";
        }
    }

    public class LeftSideDialogOptions : SideDialogOptions
    {
        public LeftSideDialogOptions()
        {
            Width = "90%";
            Position = DialogPosition.Left;
            ShowClose = true;
        }
    }

    public class FullScreenDialogOptions : DialogOptions
    {
        public FullScreenDialogOptions()
        {
            Width = "95%";
            Height = "95%";
            ShowClose = true;
        }
    }
}
