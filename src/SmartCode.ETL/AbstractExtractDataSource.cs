using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.ETL.Entity;
using SmartSql;

namespace SmartCode.ETL
{
    public abstract class AbstractExtractDataSource : IDataSource, IExtractData
    {
        private readonly Project _project;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ExtractDataSource> _logger;
        private readonly IPluginManager _pluginManager;
        public abstract string Name { get; }
        protected ISqlMapper SqlMapper { get; private set; }

        public bool Initialized { get; private set; }

        private IETLTaskRepository _etlRepository;

        protected AbstractExtractDataSource(Project project
            , ILoggerFactory loggerFactory
            , ILogger<ExtractDataSource> logger
            , IPluginManager pluginManager)
        {
            _project = project;
            _loggerFactory = loggerFactory;
            _logger = logger;
            _pluginManager = pluginManager;
            _etlRepository = _pluginManager.Resolve<IETLTaskRepository>(_project.GetETLRepository());
        }

        public Task InitData()
        {
            return Task.CompletedTask;
        }

        private async Task Extract()
        {
            var queryParams = new Dictionary<string, object>
            {
                {"LastMaxId", LastExtract.MaxId},
                {"LastQueryTime", LastExtract.QueryTime},
                {"LastMaxModifyTime", LastExtract.MaxModifyTime},
                {nameof(Offset), Offset},
                {nameof(BulkSize), BulkSize},
            };

            var extractEntity = new ETLExtract
            {
                QueryTime = DateTime.Now,
                PKColumn = PKColumn,
                QueryCommand = new ETLDbCommand
                {
                    Command = QueryCmd,
                    Parameters = queryParams
                },
                Count = -1,
                MaxId = -1
            };
            Stopwatch stopwatch = Stopwatch.StartNew();
            await LoadData(new RequestContext {RealSql = QueryCmd, Request = queryParams});
            stopwatch.Stop();
            extractEntity.Count = GetCount();
            extractEntity.QueryCommand.Taken = stopwatch.ElapsedMilliseconds;
            _logger.LogWarning($"InitData,Data.Size:{extractEntity.Count},Taken:{extractEntity.QueryCommand.Taken}ms!");

            if (!String.IsNullOrEmpty(PKColumn)
                && (PkIsNumeric || AutoIncrement)
                && extractEntity.Count > 0)
            {
                extractEntity.MaxId = GetMaxId(PKColumn);
            }
            else
            {
                extractEntity.MaxId = LastExtract.MaxId;
            }

            if (!String.IsNullOrEmpty(ModifyTime)
                && extractEntity.Count > 0)
            {
                extractEntity.MaxModifyTime = GetMaxModifyTime(ModifyTime);
            }
            else
            {
                extractEntity.MaxModifyTime = LastExtract.MaxModifyTime;
            }

            await _etlRepository.Extract(_project.GetETKTaskId(), extractEntity);
        }

        protected abstract Task LoadData(RequestContext requestContext);
        public abstract int GetCount();
        public abstract long GetMaxId(string pkColumn);
        public abstract DateTime GetMaxModifyTime(string modifyTime);

        public void Initialize(IDictionary<string, object> parameters)
        {
            Initialized = true;
        }

        public long Total { get; private set; }
        public int BulkSize { get; private set; }
        public int Offset { get; private set; }

        public String PKColumn { get; private set; }
        public String ModifyTime { get; private set; }
        public String QueryCmd { get; private set; }
        public String TotalCmd { get; private set; }
        public bool PkIsNumeric { get; private set; }
        public bool AutoIncrement { get; private set; }
        protected ETLExtract LastExtract { get; private set; }

        public async Task Run()
        {
            LastExtract = await _etlRepository.GetLastExtract(_project.GetETLCode());
            _project.SetETLLastExtract(LastExtract);
            InitParameters();

            var queryParams = new Dictionary<string, object>
            {
                {"LastMaxId", LastExtract.MaxId},
                {"LastQueryTime", LastExtract.QueryTime},
                {"LastMaxModifyTime", LastExtract.MaxModifyTime}
            };

            if (!String.IsNullOrEmpty(TotalCmd))
            {
                Total = await SqlMapper.ExecuteScalarAsync<long>(new RequestContext
                {
                    RealSql = TotalCmd,
                    Request = queryParams
                });
                if (Total == 0)
                {
                    _logger.LogInformation("can not find any record.");
                    return;
                }
            }


            BuildContext buildContext = null;
            while (Offset < Total)
            {
                _logger.LogInformation($"-------- Offset:[{Offset}]  ---------");
                try
                {
                    var etlTaskId = await _etlRepository.Startup(_project.ConfigPath, _project.GetETLCode());
                    _project.SetETKTaskId(etlTaskId);
                    await Extract();
                    foreach (var buildKV in _project.BuildTasks)
                    {
                        _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
                        var output = buildKV.Value.Output;
                        buildContext = new BuildContext
                        {
                            PluginManager = _pluginManager,
                            DataSource = this,
                            Project = _project,
                            BuildKey = buildKV.Key,
                            Build = buildKV.Value,
                            Output = output?.Copy()
                        };
                        await _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext);
                        _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
                    }

                    await _etlRepository.Success(_project.GetETKTaskId());
                    Offset += BulkSize;
                }
                catch (Exception e)
                {
                    await _etlRepository.Fail(_project.GetETKTaskId(), e);
                    throw;
                }
            }
        }

        private void InitParameters()
        {
            var dataSource = _project.DataSource;
            dataSource.Parameters.EnsureValue("DbProvider", out string dbProvider);
            dataSource.Parameters.EnsureValue("ConnectionString", out String connString);
            dataSource.Parameters.Value("PKColumn", out string pkColumn);
            PKColumn = pkColumn;
            dataSource.Parameters.Value("ModifyTime", out string modifyTime);
            ModifyTime = modifyTime;
            dataSource.Parameters.Value("PkIsNumeric", out bool pkIsNumeric);
            PkIsNumeric = pkIsNumeric;
            dataSource.Parameters.Value("AutoIncrement", out bool autoIncrement);
            AutoIncrement = autoIncrement;
            dataSource.Parameters.EnsureValue("Query", out string queryCmd);
            QueryCmd = queryCmd;
            dataSource.Parameters.Value("Total", out String totalCmd);
            TotalCmd = totalCmd;
            _project.DataSource.Parameters.EnsureValue("BulkSize", out int bulkSize);
            BulkSize = bulkSize;

            #region CreateSqlMapper

            SqlMapper = new SmartSqlBuilder()
                .UseAlias(Name)
                .UseLoggerFactory(_loggerFactory)
                .UseDataSource(dbProvider, connString)
                .Build().SqlMapper;

            #endregion
        }
    }
}