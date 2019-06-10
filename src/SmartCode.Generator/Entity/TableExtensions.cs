namespace SmartCode.Generator.Entity {

    public static class TableExtensions {

        /// <summary>
        /// 获取表的摘要说明，如果 Description 不为空， 则返回 Description；
        /// 否则返回 Type + Name
        /// </summary>
        public static string GetSummary(this Table table) {
            if (!string.IsNullOrEmpty(table.Description)) {
                return table.Description;
            }
            return table.Type + ", " + table.Name;
        }
    }
}
