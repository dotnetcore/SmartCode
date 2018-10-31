using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.ETL.Entity
{
    public class ETLTask
    {
        public long Id { get; set; }
        /// <summary>
        /// 配置文件地址
        /// </summary>
        public String ConfigPath { get; set; }
        /// <summary>
        /// ETL任务类型唯一性编码
        /// </summary>
        public String Code { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 数据抽取
        /// </summary>
        public ETLExtract Extract { get; set; }
        /// <summary>
        /// 数据转换
        /// </summary>
        public ETLTransform Transform { get; set; }
        /// <summary>
        /// 数据加载
        /// </summary>
        public ETLLoad Load { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public IDictionary<String, object> ExtendData { get; set; }
        /// <summary>
        /// ETL任务状态
        /// </summary>
        public ETLTaskStatus Status { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// ETL完成时间
        /// </summary>
        public DateTime CompletedTime { get; set; }
        #region DB
        public DateTime ModifyTime { get; set; }
        public DateTime CreateTime { get; set; }
        #endregion
    }
    [Flags]
    public enum ETLTaskStatus
    {
        Startup = 0,
        Extracted = 1 << 0,
        Transformed = 1 << 1,
        Loaded = 1 << 2,
        Succeed = 1 << 8,
        Failed = 1 << 10
    }
}
