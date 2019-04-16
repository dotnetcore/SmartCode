using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode
{
    public class NoneDataSource : IDataSource
    {
        public bool Initialized => true;

        public string Name => "None";

        public Task InitData()
        {
            return Task.CompletedTask;
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
