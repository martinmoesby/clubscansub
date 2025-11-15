using ClubScansub.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Utility.Extensions
{
    public static class IconSizeExtensions
    {
        public static string ToFontSizeStyle(this IconSizeEnum size)
        {
            return size switch
            {
                IconSizeEnum.ExtraSmall => "font-size: 0.75rem;",
                IconSizeEnum.Small => "font-size: 0.875rem;",
                IconSizeEnum.Medium => "font-size: 1rem;",
                IconSizeEnum.Large => "font-size: 1.25rem;",
                _ => "font-size: 1rem;",
            };
        }

        public static string ToCssClass(this IconSizeEnum size)
        {
            return size switch
            {
                IconSizeEnum.ExtraSmall => "icon-xs",
                IconSizeEnum.Small => "icon-sm",
                IconSizeEnum.Medium => "icon-md",
                IconSizeEnum.Large => "icon-lg",
                _ => "icon-md",
            };
        }
    }
}
