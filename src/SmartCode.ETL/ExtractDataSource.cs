using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartSql.Abstractions;
using SmartSql.Batch;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SmartCode.Db.SmartSqlMapperFactory;
using System.Data;
using System.Diagnostics;
using SmartSql;

namespace SmartCode.ETL
{
    public class ExtractDataSource : IDataSource
    {
        private readonly Project _project;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ExtractDataSource> _logger;

        public bool Initialized => true;
        public string Name => "Extract";
        public DbTable Data { get; set; }
        public DbTable TransformData { get; set; }
        public ExtractDataSource(Project project, ILoggerFactory loggerFactory, ILogger<ExtractDataSource> logger)
        {
            _project = project;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }
        public async Task InitData()
        {
            var dataSource = _project.DataSource;
            dataSource.Paramters.EnsureValue("DbProvider", out string dbProvider);
            dataSource.Paramters.EnsureValue("ConnectionString", out string connString);
            var smartSqlOptions = new CreateSmartSqlMapperOptions
            {
                LoggerFactory = _loggerFactory,
                ProviderName = dbProvider,
                Alias = Name,
                DataSource = new SmartSql.Configuration.WriteDataSource
                {
                    Name = Name,
                    ConnectionString = connString
                }
            };
            var sqlMapper = Create(smartSqlOptions);
            Stopwatch stopwatch = Stopwatch.StartNew();
            dataSource.Paramters.EnsureValue("Query", out string queryCmd);
            Data = await sqlMapper.GetDbTableAsync(new RequestContext { RealSql = queryCmd });
            stopwatch.Stop();
            _logger.LogWarning($"InitData,Data.Size:{Data.Rows.Count},Taken:{stopwatch.ElapsedMilliseconds}ms!");
        }

        public void Initialize(IDictionary<string, object> paramters)
        {

        }
    }
}
