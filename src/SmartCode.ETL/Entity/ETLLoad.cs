using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class ETLLoad
    {
        public ETLDbCommand PreCommand { get; set; }
        public ETLDbCommand PostCommand { get; set; }
        public int Size { get; set; }
        public long Taken { get; set; }
        public string Table { get; set; }
        public IDictionary<String, object> Parameters { get; set; }
    }
}
