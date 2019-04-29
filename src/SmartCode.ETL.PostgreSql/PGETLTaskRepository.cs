using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartCode.ETL.Entity;
using SmartCode.Db;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Options;

namespace SmartCode.ETL.PostgreSql
{
    public class PGETLTaskRepository : IETLTaskRepository
    {
        private const string CONNECTION_STRING = "ConnectionString";
        private readonly ILoggerFactory _loggerFactory;
        private const string DEFAULT_SQLMAP_PATH = "PGETL_SqlMaps";
        public bool Initialized { get; private set; }
        public string Name => "PG";
        public string Scope => "EtlTask";
        public ISqlMapper SqlMapper { get; set; }
        public PGETLTaskRepository(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public void Initialize(IDictionary<string, object> paramters)
        {
            paramters.EnsureValue(CONNECTION_STRING, out string connectionString);

            SqlMapper = SmartSqlMapperFactory.Create(new SmartSqlMapperFactory.CreateSmartSqlMapperOptions
            {
                Alias = "PGETLRepository",
                LoggerFactory = _loggerFactory,
                ProviderName = "PostgreSql",
                SqlMapPath = DEFAULT_SQLMAP_PATH,
                DataSource = new DataSource
                {
                    ConnectionString = connectionString,
                    Name = "PGETL"
                }
            });
            Initialized = true;
        }
        public Task Extract(long etlTaskId, ETLExtract extract)
        {
            return SqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Update",
                Request = new
                {
                    Id = etlTaskId,
                    Status = ETLTaskStatus.Extracted,
                    Extract = extract
                }
            });
        }

        public Task Fail(long etlTaskId, Exception errorException)
        {
            return SqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Update",
                Request = new
                {
                    Id = etlTaskId,
                    Status = ETLTaskStatus.Failed,
                    ExtendData = new Dictionary<string, object>
                    {
                        { "error_msg",errorException.Message},
                        { "stack_trace",errorException.StackTrace}
                    }
                }
            });
        }

        public async Task<ETLExtract> GetLastExtract(string code)
        {
            var etlTaskEntity = await SqlMapper.QuerySingleAsync<ETLTask>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetLastExtract",
                Request = new
                {
                    Code = code
                }
            });
            if (etlTaskEntity == null)
            {
                return ETLExtract.Default;
            }
            if (etlTaskEntity.Extract.MaxModifyTime == DateTime.MinValue)
            {
                etlTaskEntity.Extract.MaxModifyTime = ETLExtract.MinDateTime;
            }
            return etlTaskEntity.Extract;
        }

        public Task<ETLTask> GetLastTask(string code)
        {
            return SqlMapper.QuerySingleAsync<ETLTask>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetLastTask",
                Request = new
                {
                    Code = code
                }
            });
        }

        public Task Load(long etlTaskId, ETLLoad load)
        {
            return SqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Update",
                Request = new
                {
                    Id = etlTaskId,
                    Status = ETLTaskStatus.Loaded,
                    Load = load
                }
            });
        }

        public Task<long> Startup(string configPath, string code)
        {
            code = code ?? configPath;
            return SqlMapper.QuerySingleAsync<long>(new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                Request = new ETLTask
                {
                    ConfigPath = configPath,
                    Code = code,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    StartTime = DateTime.Now,
                    Version = 1,
                    Status = ETLTaskStatus.Startup
                }
            });
        }

        public Task Success(long etlTaskId)
        {
            return SqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Update",
                Request = new
                {
                    Id = etlTaskId,
                    Status = ETLTaskStatus.Succeed,
                    CompletedTime = DateTime.Now
                }
            });
        }

        public Task Transform(long etlTaskId, ETLTransform transform)
        {
            return SqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Update",
                Request = new
                {
                    Id = etlTaskId,
                    Status = ETLTaskStatus.Transformed,
                    Transform = transform
                }
            });
        }
    }
}
