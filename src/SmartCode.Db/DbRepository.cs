using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Options;

namespace SmartCode.Db
{
    public class DbRepository : IDbRepository
    {
        private readonly IDictionary<string, SmartSql.Configuration.DbProvider> _dbProviders = new Dictionary<string, SmartSql.Configuration.DbProvider>();
        private readonly DataSource _dataSource;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DbRepository> _logger;

        public ISmartSqlMapper SqlMapper { get; private set; }
        public string DbProviderName { get; private set; }
        public DbProvider DbProvider { get { return (DbProvider)Enum.Parse(typeof(DbProvider), DbProviderName); } }
        public string DbName { get; private set; }
        public string ConnectionString { get; private set; }
        public String MapPath { get; private set; } = "Maps";
        public DbRepository(
            DataSource dataSource
            , ILoggerFactory loggerFactory)
        {
            _dataSource = dataSource;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DbRepository>();
            InitDataSource();
            InitSqlMapper();
        }
        private void InitDataSource()
        {
            var dataSource = _dataSource;
            DbProviderName = dataSource.Paramters["DbProvider"].ToString();
            DbName = dataSource.Paramters["DbName"].ToString();
            ConnectionString = dataSource.Paramters["ConnectionString"].ToString();
            if (dataSource.Paramters.Value("MapPath", out string mapPath))
            {
                MapPath = mapPath;
            }
        }

        private void InitSqlMapper()
        {
            var smartSqlDbProvider = DbProviders.GetDbProvider(DbProviderName);
            SmartSqlConfigOptions smartSqlConfigOptions = new SmartSqlConfigOptions
            {
                Settings = new SmartSql.Configuration.Settings
                {
                    ParameterPrefix = "$"
                },
                Database = new Database
                {
                    DbProvider = smartSqlDbProvider,
                    Write = new SmartSql.Configuration.WriteDataSource
                    {
                        Name = DbName,
                        ConnectionString = ConnectionString
                    },
                    Read = new List<SmartSql.Configuration.ReadDataSource>()
                },
                SmartSqlMaps = new List<SmartSql.Configuration.SmartSqlMapSource> {
                          new SmartSql.Configuration.SmartSqlMapSource
                          {
                               Path= MapPath,
                               Type= SmartSql.Configuration.SmartSqlMapSource.ResourceType.Directory
                          }
                     },
                TypeHandlers = new List<SmartSql.Configuration.TypeHandler>()
            };

            var _configLoader = new OptionConfigLoader(smartSqlConfigOptions, _loggerFactory);
            var smartsqlOptions = new SmartSqlOptions
            {
                ConfigPath = "SmartSql",
                ConfigLoader = _configLoader
            };
            SqlMapper = MapperContainer.Instance.GetSqlMapper(smartsqlOptions);
        }
    }
}
