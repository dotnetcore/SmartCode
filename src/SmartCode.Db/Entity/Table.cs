using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCode.Db.Entity
{
    public class Table : IConvertedName
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public String Name { get; set; }
        public String TypeName { get; set; }
        public TableType Type
        {
            get
            {
                switch (TypeName.Trim())
                {
                    case "T": return TableType.Table;
                    case "V": return TableType.View;
                    default: throw new ArgumentException("参数错误！", "Table.TypeName");
                }
            }
        }
        public Column PKColumn { get { return Columns.FirstOrDefault(m => m.IsPrimaryKey); } }
        public bool AutoIncrement { get { return Columns.Any(m => m.AutoIncrement); } }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public enum TableType
        {
            Table,
            View
        }
        public String ConvertedName { get; set; }
    }
}
