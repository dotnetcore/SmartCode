using System;
using System.Collections.Generic;

namespace SmartCode.Configuration
{
    public class TableFilter
    {
        public IEnumerable<String> IncludeTables { get; set; }
        public IEnumerable<String> IgnoreTables { get; set; }
        public bool? IgnoreNoPKTable { get; set; }
        public bool? IgnoreView { get; set; }
    }
}