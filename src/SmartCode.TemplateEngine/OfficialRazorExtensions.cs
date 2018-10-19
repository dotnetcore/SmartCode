using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class OfficialRazorExtensions
    {
        public static String NewLine(this IHtmlHelper htmlHelper)
        {
            return Environment.NewLine;
        }
        public static String NewLine(this IHtmlHelper htmlHelper, string appendStr)
        {
            return Environment.NewLine + appendStr;
        }

        public static String PadLeft(this IHtmlHelper htmlHelper, int totalWidth)
        {
            return String.Empty.PadLeft(totalWidth);
        }
        public static String PadRight(this IHtmlHelper htmlHelper, int totalWidth)
        {
            return String.Empty.PadRight(totalWidth);
        }
    }
}
