using System;
using System.Text;
using SmartCode.Generator.Entity;

namespace SmartCode.Generator.Extensions
{
    public static class SummaryExtensions
    {
        public static string GetCSharpSummary(this Column column)
        {
            var summary = column.GetSummary();
            return GetCSharpSummary(summary);
        }
        
        public static string GetCSharpSummary(this Table table)
        {
            var summary = table.GetSummary();
            return GetCSharpSummary(summary);
        }
        public static string GetCSharpSummary(this string summary)
        {
            StringBuilder csharpSummary = new StringBuilder();
            csharpSummary.AppendLine("///<summary>");
            if (summary.Contains(Environment.NewLine))
            {
                foreach (var summaryLine in summary.Split(Environment.NewLine.ToCharArray()))
                {
                    if (String.IsNullOrEmpty(summaryLine))
                    {
                        continue;
                    }

                    csharpSummary.AppendLine($"/// {summaryLine}");
                }
            }
            else
            {
                csharpSummary.AppendLine($"/// {summary}");
            }

            csharpSummary.Append("///</summary>");
            return csharpSummary.ToString();
        }
    }
}