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
        private readonly DataSource _dataSource;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DbRepository> _logger;

        public ISmartSqlMapper SqlMapper { get; private set; }
        public string DbProviderName { get; private set; }
        public DbProvider DbProvider { get { return (DbProvider)Enum.Parse(typeof(DbProvider), DbProviderName); } }
        public string DbName { get; private set; }
        public string DbSchema { get; protected set; }
        public string ConnectionString { get; private set; }
        public String SqlMapPath { get; private set; } = "Maps";
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
            dataSource.Paramters.EnsureValue("DbProvider", out string dbProvider);
            DbProviderName = dbProvider;
            dataSource.Paramters.EnsureValue("DbName", out string dbName);
            DbName = dbName;
            dataSource.Paramters.EnsureValue("ConnectionString", out string connectionString);
            ConnectionString = connectionString;
            if (dataSource.Paramters.Value("DbSchema", out string dbSchema))
            {
                DbSchema = dbSchema;
            }
            if (dataSource.Paramters.Value("SqlMapPath", out string mapPath))
            {
                SqlMapPath = mapPath;
            }
        }

        private void InitSqlMapper()
        {
            SqlMapper = SmartSqlMapperFactory.Create(new SmartSqlMapperFactory.CreateSmartSqlMapperOptions
            {
                DataSource = new SmartSql.Configuration.WriteDataSource
                {
                    Name = DbName,
                    ConnectionString = ConnectionString
                },
                LoggerFactory = _loggerFactory,
                ProviderName = DbProviderName,
                SqlMapPath = SqlMapPath
            });
        }
    }
}
