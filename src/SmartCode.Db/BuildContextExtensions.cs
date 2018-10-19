using SmartCode.Db.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public  static class BuildContextExtensions
    {
        public const String CURRENT_ALL_TABLE = "CurrnetAllTable";
        public const String CURRENT_TABLE = "CurrentTable";
        public static IEnumerable<Table> GetCurrentAllTable(this BuildContext context)
        {
            return context.GetItem<IEnumerable<Table>>(CURRENT_ALL_TABLE);
        }
        public static void SetCurrentAllTable(this BuildContext context, IEnumerable<Table> tables)
        {
            context.SetItem(CURRENT_ALL_TABLE, tables);
        }
        public static Table GetCurrentTable(this BuildContext context)
        {
            return context.GetItem<Table>(CURRENT_TABLE);
        }
        public static void SetCurrentTable(this BuildContext context, Table table)
        {
            context.SetItem(CURRENT_TABLE, table);
        }
    }
}
