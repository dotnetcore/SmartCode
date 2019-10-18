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

        /// <summary>
        /// 文件创建模式
        /// </summary>
        public CreateMode Mode { get; set; } = CreateMode.None;

        /// <summary>
        /// 文件名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// '.' 拆分成 '/'
        /// </summary>
        public bool? DotSplit { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public String Extension { get; set; }

        public Output Copy()
        {
            return new Output
            {
                Type = Type,
                Path = Path,
                DotSplit = DotSplit,
                Name = Name,
                Mode = Mode,
                Extension = Extension
            };
        }
    }

    /// <summary>
    /// 文件创建模式
    /// </summary>
    public enum CreateMode
    {
        None,

        /// <summary>
        /// 增量创建，如果存在则忽略
        /// </summary>
        Incre,

        /// <summary>
        /// 全量创建，如果存在则重新创建
        /// </summary>
        Full
    }
}