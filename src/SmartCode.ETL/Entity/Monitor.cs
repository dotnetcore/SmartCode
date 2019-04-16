using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class Monitor
    {
        public string BuildKey { get; set; }

        public string MachineName { get; }

        public string ProcessName { get; }

        public long NonpagedSystemMemorySize64 { get; }
        public long PagedMemorySize64 { get; }
        public long PagedSystemMemorySize64 { get; }
        public long WorkingSet64 { get; }
        public long VirtualMemorySize64 { get; }
        public TimeSpan UserProcessorTime { get; }
        public TimeSpan TotalProcessorTime { get; }
        
        public TimeSpan PrivilegedProcessorTime { get; }
        public long PrivateMemorySize64 { get; }
        public bool PriorityBoostEnabled { get; set; }
        public long PeakWorkingSet64 { get; }
        public long PeakVirtualMemorySize64 { get; }
        public long PeakPagedMemorySize64 { get; }
    }
}
