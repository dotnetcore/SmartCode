using RazorLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace RazorLight
{
    public static class RazorLightExtensions
    {
        public static string NewLine(this ITemplatePage page)
        {
            return Environment.NewLine;
        }
        public static string PadLeft(this ITemplatePage page,int totalWidth)
        {
           return String.Empty.PadLeft(totalWidth);
        }
        public static string PadRight(this ITemplatePage page, int totalWidth)
        {
            return String.Empty.PadRight(totalWidth);
        }
    }
}
