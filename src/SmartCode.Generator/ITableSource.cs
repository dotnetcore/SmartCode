using System.Collections.Generic;
using SmartCode.Generator.Entity;

namespace SmartCode.Generator
{
    public interface ITableSource : IDataSource
    {
        IList<Table> Tables { get; }
    }
}