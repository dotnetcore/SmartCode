using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db.Entity;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Options;

namespace SmartCode.Db
{
    public class DbRepository : IDbRepository
    {
        private readonly Project _project;
        private readonly IDictionary<string, SmartSql.Configuration.DbProvider> _dbProviders = new Dictionary<string, SmartSql.Configuration.DbProvider>();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DbRepository> _logger;

        public String Scope => "Database";
        public ISmartSqlMapper SqlMapper { get; private set; }
        public string DbProviderName { get; private set; }
        public DbProvider DbProvider { get { return (DbProvider)Enum.Parse(typeof(DbProvider), DbProviderName); } }
        public string DbName { get; private set; }
        public string ConnectionString { get; private set; }
        public DbRepository(
            Project project
            , ILoggerFactory loggerFactory)
        {
            _project = project;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DbRepository>();
            InitDbProviders();
            InitDataSource();
            InitSqlMapper();
        }
        private void InitDbProviders()
        {
            _dbProviders.Add("MySql", new SmartSql.Configuration.DbProvider
            {
                Name = "MySql",
                ParameterPrefix = "?",
                Type = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data"
            });
            _dbProviders.Add("MariaDB", new SmartSql.Configuration.DbProvider
            {
                Name = "MariaDB",
                ParameterPrefix = "?",
                Type = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data"
            });
            _dbProviders.Add("PostgreSql", new SmartSql.Configuration.DbProvider
            {
                Name = "PostgreSql",
                ParameterPrefix = "@",
                Type = "Npgsql.NpgsqlFactory,Npgsql"
            });
            _dbProviders.Add("SqlServer", new SmartSql.Configuration.DbProvider
            {
                Name = "SqlServer",
                ParameterPrefix = "@",
                Type = "System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient"
            });
            _dbProviders.Add("Oracle", new SmartSql.Configuration.DbProvider
            {
                Name = "Oracle",
                ParameterPrefix = ":",
                Type = "Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess"
            });
            _dbProviders.Add("SQLite", new SmartSql.Configuration.DbProvider
            {
                Name = "SQLite",
                ParameterPrefix = "$",
                Type = "System.Data.SQLite.SQLiteFactory,System.Data.SQLite"
            });
        }
        private void InitDataSource()
        {
            var dataSource = _project.DataSource;
            DbProviderName = dataSource.Paramters["DbProvider"].ToString();
            DbName = dataSource.Paramters["DbName"].ToString();
            ConnectionString = dataSource.Paramters["ConnectionString"].ToString();
        }

        private void InitSqlMapper()
        {
            if (!_dbProviders.TryGetValue(DbProviderName, out SmartSql.Configuration.DbProvider smartSqlDbProvider))
            {
                var supportDbProviders = String.Join(",", _dbProviders.Select(m => m.Key));
                var errMsg = $"Can not find DbProvider:{DbProviderName},SmartCode support DbProviders:{supportDbProviders}!";
                _logger.LogError(errMsg);
                throw new SmartCodeException(errMsg);
            }
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
                               Path="Maps",
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

        public IEnumerable<Table> QueryTable()
        {
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName}, QueryTable Start! ----");
            IEnumerable<Table> tables;
            try
            {
                SqlMapper.BeginSession();
                tables = SqlMapper.Query<Table>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "QueryTable",
                    Request = new { DBName = DbName }
                });
                foreach (var table in tables)
                {
                    table.Columns = SqlMapper.Query<Column>(new RequestContext
                    {
                        Scope = Scope,
                        SqlId = "QueryColumn",
                        Request = new { DBName = DbName, TableId = table.Id, TableName = table.Name }
                    });
                }
            }
            finally
            {
                SqlMapper.EndSession();
            }
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName},Tables:{tables.Count()} QueryTable End! ----");
            return tables;
        }
    }
}
