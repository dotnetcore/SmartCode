using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartCode.ETL.Entity;

namespace SmartCode.ETL
{
    public class NoneETLTaskRepository : IETLTaskRepository
    {
        public bool Initialized => true;

        public string Name => "None";

        public Task Extract(long etlTaskId, ETLExtract extract)
        {
            return Task.CompletedTask;
        }
        public Task Fail(long etlTaskId, Exception errorException)
        {
            return Task.CompletedTask;
        }

        public Task<ETLExtract> GetLastExtract(string code)
        {
            return Task.FromResult(ETLExtract.Default);
        }

        public Task<ETLTask> GetLastTask(string code)
        {
            return Task.FromResult(new ETLTask());
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }

        public Task Load(long etlTaskId, ETLLoad load)
        {
            return Task.CompletedTask;
        }

        public Task<long> Startup(string configPath, string code)
        {
            return Task.FromResult<long>(-1);
        }

        public Task Success(long etlTaskId)
        {
            return Task.CompletedTask;
        }

        public Task Transform(long etlTaskId, ETLTransform transform)
        {
            return Task.CompletedTask;
        }
    }
}
