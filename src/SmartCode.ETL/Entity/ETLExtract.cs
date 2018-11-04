using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class ETLExtract
    {
        public static ETLExtract Default = new ETLExtract
        {
            MaxId = -1,
            QueryTime = DateTime.Parse("1970-01-01 08:00:00"),
            QuerySize = -1
        };
        public string PKColumn { get; set; }
        public long MaxId { get; set; }
        public DateTime MaxModifyTime { get; set; }
        public DateTime QueryTime { get; set; }
        public ETLDbCommand QueryCommand { get; set; }
        public int QuerySize { get; set; }
    }
}
