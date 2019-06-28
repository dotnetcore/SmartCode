using System;
using System.Text;

namespace SmartCode.Generator.Entity
{
    public static class ColumnExtensions
    {
        /// <summary>
        /// 获取列的摘要说明，如果 Description 不为空， 则返回 Description；
        /// 否则返回 Name + DbType
        /// </summary>
        public static string GetSummary(this Column column)
        {
            if (!string.IsNullOrEmpty(column.Description))
            {
                return column.Description;
            }

            return column.Name + ", " + column.DbType;
        }
    }
}