using Microsoft.Extensions.Logging;
using SmartCode.Db;
using SmartSql.Abstractions;
using SmartSql.Batch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static SmartCode.Db.SmartSqlMapperFactory;

namespace SmartCode.ETL.BuildTasks
{
    public class LoadBuildTask : IBuildTask
    {
        private const string TABLE_NAME = "Table";
        private const string DB_PROVIDER = "DbProvider";
        private const string COLUMN_MAPPING = "ColumnMapping";
        private const string PRE_COMMAND = "PreCommand";
        private const string POST_COMMAND = "PostCommand";
        private readonly ILoggerFactory _loggerFacotry;
        private readonly ILogger<LoadBuildTask> _logger;

        public bool Initialized => true;
        public string Name => "Load";
        public LoadBuildTask(ILoggerFactory loggerFacotry
            , ILogger<LoadBuildTask> logger)
        {
            _loggerFacotry = loggerFacotry;
            _logger = logger;
        }

        public async Task Build(BuildContext context)
        {
            context.Build.Paramters.EnsureValue(TABLE_NAME, out string tableName);
            context.Build.Paramters.EnsureValue(DB_PROVIDER, out DbProvider dbProvider);

            var dataSource = context.GetDataSource<ExtractDataSource>();
            var batchTable = dataSource.TransformData;
            batchTable.Name = tableName;
            var sqlMapper = GetSqlMapper(context);
            if (context.Build.Paramters.Value(PRE_COMMAND, out string preCmd) && !String.IsNullOrEmpty(preCmd))
            {
                await sqlMapper.ExecuteAsync(new RequestContext
                {
                    RealSql = PRE_COMMAND
                });
            }
            using (IBatchInsert batchInsert = BatchInsertFactory.Create(sqlMapper, dbProvider))
            {
                InitColumnMapping(batchInsert, context);
                batchInsert.Table = batchTable;
                Stopwatch stopwatch = Stopwatch.StartNew();
                await batchInsert.InsertAsync();
                stopwatch.Stop();
                _logger.LogWarning($"Build:{context.BuildKey},BatchInsert.Size:{batchTable.Rows.Count},Taken:{stopwatch.ElapsedMilliseconds}ms!");
            }
            if (context.Build.Paramters.Value(POST_COMMAND, out string postCmd) && !String.IsNullOrEmpty(postCmd))
            {
                await sqlMapper.ExecuteAsync(new RequestContext
                {
                    RealSql = postCmd
                });
            }
        }

        public void Initialize(IDictionary<string, object> paramters)
        {

        }

        private void InitColumnMapping(IBatchInsert batchInsert, BuildContext context)
        {
            if (context.Build.Paramters.Value(COLUMN_MAPPING, out IEnumerable colMapps))
            {
                foreach (IDictionary<object, object> colMappingKV in colMapps)
                {
                    colMappingKV.EnsureValue("Column", out string colName);
                    colMappingKV.EnsureValue("Mapping", out string mapping);
                    colMappingKV.Value("DataTypeName", out string dataTypeName);
                    var colMapping = new ColumnMapping
                    {
                        Column = colName,
                        Mapping = mapping,
                        DataTypeName = dataTypeName
                    };
                    batchInsert.AddColumnMapping(colMapping);
                }
            }
        }
        private ISmartSqlMapper GetSqlMapper(BuildContext context)
        {
            var smartSqlOptions = InitCreateSmartSqlMapperOptions(context);
            return SmartSqlMapperFactory.Create(smartSqlOptions);
        }
        private CreateSmartSqlMapperOptions InitCreateSmartSqlMapperOptions(BuildContext context)
        {
            context.Build.Paramters.EnsureValue(DB_PROVIDER, out string dbProvider);
            context.Build.Paramters.EnsureValue("ConnectionString", out string connString);
            var alias_name = $"{Name}_{context.BuildKey}";
            return new CreateSmartSqlMapperOptions
            {
                Alias = alias_name,
                LoggerFactory = _loggerFacotry,
                ProviderName = dbProvider,
                DataSource = new SmartSql.Configuration.WriteDataSource
                {
                    Name = Name,
                    ConnectionString = connString
                }
            };
        }
    }
}
