using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db;
using SmartSql.Bulk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using SmartSql;
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<LoadBuildTask> _logger;

        public bool Initialized => true;
        public string Name => "Load";

        public LoadBuildTask(ILoggerFactory loggerFactory
            , Project project
            , IPluginManager pluginManager
            , ILogger<LoadBuildTask> logger)
        {
            _loggerFactory = loggerFactory;
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public async Task Build(BuildContext context)
        {
            context.Build.Parameters.EnsureValue(TABLE_NAME, out string tableName);
            context.Build.Parameters.EnsureValue(DB_PROVIDER, out DbProvider dbProvider);
            var etlRepository = _pluginManager.Resolve<IETLTaskRepository>(_project.GetETLRepository());
            var dataSource = context.GetExtractData<ExtractData>();
            if (dataSource.TransformData.Rows.Count == 0)
            {
                await etlRepository.Load(_project.GetETKTaskId(), new Entity.ETLLoad
                {
                    Size = 0,
                    Table = tableName
                });
                return;
            }

            var batchTable = dataSource.TransformData;
            batchTable.TableName = tableName;

            var sqlMapper = GetSqlMapper(context);
            context.Build.Parameters.Value(PRE_COMMAND, out string preCmd);
            var lastExtract = _project.GetETLLastExtract();
            var queryParams = new Dictionary<string, object>
            {
                {"LastMaxId", lastExtract.MaxId},
                {"LastQueryTime", lastExtract.QueryTime},
                {"LastMaxModifyTime", lastExtract.MaxModifyTime},
            };
            Stopwatch stopwatch = Stopwatch.StartNew();
            var loadEntity = new Entity.ETLLoad
            {
                Table = tableName
            };
            try
            {
                sqlMapper.SessionStore.Open();

                #region PreCmd

                if (!String.IsNullOrEmpty(preCmd))
                {
                    stopwatch.Restart();
                    await sqlMapper.ExecuteAsync(new RequestContext
                    {
                        RealSql = preCmd,
                        Request = queryParams
                    });
                    stopwatch.Stop();
                    loadEntity.PreCommand = new Entity.ETLDbCommand
                    {
                        Command = preCmd,
                        Parameters = queryParams,
                        Taken = stopwatch.ElapsedMilliseconds
                    };
                }

                #endregion

                #region BatchInsert

                var batchInsert = BatchInsertFactory.Create(sqlMapper, dbProvider, context);
                InitColumnMapping(batchTable, context);
                batchInsert.Table = batchTable;
                stopwatch.Restart();
                await batchInsert.InsertAsync();
                stopwatch.Stop();
                loadEntity.Size = batchTable.Rows.Count;
                loadEntity.Taken = stopwatch.ElapsedMilliseconds;
                _logger.LogWarning(
                    $"Build:{context.BuildKey},BatchInsert.Size:{loadEntity.Size},Taken:{loadEntity.Taken}ms!");

                #endregion

                #region PostCmd

                if (context.Build.Parameters.Value(POST_COMMAND, out string postCmd) && !String.IsNullOrEmpty(postCmd))
                {
                    stopwatch.Restart();
                    await sqlMapper.ExecuteAsync(new RequestContext
                    {
                        RealSql = postCmd,
                        Request = queryParams
                    });
                    stopwatch.Stop();
                    loadEntity.PostCommand = new Entity.ETLDbCommand
                    {
                        Command = postCmd,
                        Parameters = queryParams,
                        Taken = stopwatch.ElapsedMilliseconds
                    };
                }

                #endregion

                await etlRepository.Load(_project.GetETKTaskId(), loadEntity);
            }
            finally
            {
                sqlMapper.SessionStore.Dispose();
            }
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
        }

        private void InitColumnMapping(DataTable bulkTable, BuildContext context)
        {
            if (context.Build.Parameters.Value(COLUMN_MAPPING, out IEnumerable colMapps))
            {
                foreach (IDictionary<object, object> colMappingKV in colMapps)
                {
                    colMappingKV.EnsureValue("Column", out string colName);
                    colMappingKV.EnsureValue("Mapping", out string mapping);
                    var sourceColumn = bulkTable.Columns[colName];
                    sourceColumn.ColumnName = mapping;
                    if (colMappingKV.Value("DataTypeName", out string dataTypeName))
                    {
                        sourceColumn.ExtendedProperties.Add("DataTypeName", dataTypeName);
                    }
                }
            }
        }

        private ISqlMapper GetSqlMapper(BuildContext context)
        {
            context.Build.Parameters.EnsureValue(DB_PROVIDER, out string dbProvider);
            context.Build.Parameters.EnsureValue("ConnectionString", out string connString);
            var alias_name = $"{Name}_{context.BuildKey}_{Guid.NewGuid():N}";

            return new SmartSqlBuilder()
                .UseDataSource(dbProvider, connString)
                .UseLoggerFactory(_loggerFactory)
                .UseAlias(alias_name)
                .Build().SqlMapper;
        }
    }
}