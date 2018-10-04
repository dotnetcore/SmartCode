using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Configuration
{
    public class Output
    {
        /// <summary>
        /// 输出插件类型
        /// </summary>
        public String Type { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public String Path { get; set; }
        public String Name { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public String Extension { get; set; }
    }
}
