using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCode.Configuration
{
    public class DataSource
    {
        public String Name { get; set; }
        public IDictionary<String, object> Parameters { get; set; }
    }
}
