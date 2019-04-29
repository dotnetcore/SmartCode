using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class ETLExtract
    {
        public static DateTime MinDateTime = DateTime.Parse("1970-01-01 08:00:00");

        public static ETLExtract Default = new ETLExtract
        {
            MaxId = -1,
            QueryTime = MinDateTime,
            MaxModifyTime = MinDateTime,
            Count = -1
        };
        public string PKColumn { get; set; }
        public int Count { get; set; }
        public long MaxId { get; set; }
        public DateTime MaxModifyTime { get; set; }
        public DateTime QueryTime { get; set; }
        public ETLDbCommand QueryCommand { get; set; }
        
    }
}
