using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class ETLDbCommand
    {
        public string Command { get; set; }
        public IDictionary<String, object> Parameters { get; set; }
        public long Taken { get; set; }
    }
}
