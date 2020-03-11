using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public  static class BuildContextExtensions
    {
        public static TDataSource GetDataSource<TDataSource>(this BuildContext context) where TDataSource : IDataSource
        {
            return (TDataSource)context.DataSource;
        }
    }
}
