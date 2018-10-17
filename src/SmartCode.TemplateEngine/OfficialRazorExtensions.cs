using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class OfficialRazorExtensions
    {
        public static IHtmlContent NewLine(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.Raw(Environment.NewLine);
        }

        public static IHtmlContent PadLeft(this IHtmlHelper htmlHelper, int totalWidth)
        {
            return htmlHelper.Raw(String.Empty.PadLeft(totalWidth));
        }
        public static IHtmlContent PadRight(this IHtmlHelper htmlHelper, int totalWidth)
        {
            return htmlHelper.Raw(String.Empty.PadRight(totalWidth));
        }
    }
}
