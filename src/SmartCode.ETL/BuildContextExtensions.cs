using System;
using System.Collections.Generic;
using System.Text;
using SmartSql;

namespace SmartCode.ETL
{
    public static class BuildContextExtensions
    {
        public const String ETL_EXTRACT_DATA = "ETL_ExtractData";
        public static TExtractData GetExtractData<TExtractData>(this BuildContext context)where TExtractData : IExtractData
        {
            return context.GetItem<TExtractData>(ETL_EXTRACT_DATA);
        }
        public static void SetExtractData(this BuildContext context, IExtractData extractData)
        {
            context.SetItem(ETL_EXTRACT_DATA, extractData);
        }
    }
}
