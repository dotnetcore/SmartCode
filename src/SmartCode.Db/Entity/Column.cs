using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCode.Db.Entity
{
    public class Column: IConvertedName
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public String DbType { get; set; }
        public long? DataLength { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否自增
        /// </summary>
        public bool AutoIncrement { get; set; }
        public String ConvertedName { get; set; }
        public String LanguageType { get; set; }
    }
}
