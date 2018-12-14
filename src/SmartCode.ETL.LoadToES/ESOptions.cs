using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.LoadToES
{
    public class ESOptions
    {
        public String Host { get; set; }
        public String Index { get; set; }
        public String TypeName { get; set; }
        public String Cert { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
    }

}
