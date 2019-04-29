using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartCode.ETL.Entity;

namespace SmartCode.ETL
{
    public interface IETLTaskRepository : IPlugin
    {
        Task<long> Startup(string configPath, string code);
        Task Extract(long etlTaskId, ETLExtract extract);
        Task Transform(long etlTaskId, ETLTransform transform);
        Task Load(long etlTaskId, ETLLoad load);
        Task<ETLTask> GetLastTask(string code);
        Task<ETLExtract> GetLastExtract(string code);
        Task Fail(long etlTaskId, Exception errorException);
        Task Success(long etlTaskId);
    }
}
