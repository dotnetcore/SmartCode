using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SmartCode.Configuration
{
    public class Build
    {
        /// <summary>
        /// 构建类型
        /// Table | Project | Clear
        /// </summary>
        public String Type { get; set; }
        public String Module { get; set; }
        public TemplateEngine TemplateEngine { get; set; }
        public Output Output { get; set; }
        public IEnumerable<String> IncludeTables { get; set; }
        public IEnumerable<String> IgnoreTables { get; set; }
        public bool? IgnoreNoPKTable { get; set; }
        public bool? IgnoreView { get; set; }
        public NamingConverter NamingConverter { get; set; }
        /// <summary>
        /// 依赖于
        /// </summary>
        public IEnumerable<String> DependOn { get; set; }
        /// <summary>
        /// 自定义构建参数
        /// </summary>
        public IDictionary<String, object> Parameters { get; set; }
    }
}
