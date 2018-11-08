using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Db
{
    public class SmartSqlMapperFactory
    {
        public static ISmartSqlMapper Create(CreateSmartSqlMapperOptions options)
        {
            var smartSqlDbProvider = DbProviders.GetDbProvider(options.ProviderName);
            SmartSqlConfigOptions smartSqlConfigOptions = new SmartSqlConfigOptions
            {
                Settings = new SmartSql.Configuration.Settings
                {
                    ParameterPrefix = "$",
                    IgnoreParameterCase = true,
                    IsWatchConfigFile = false,
                    IsCacheEnabled = false,
                },
                Database = new Database
                {
                    DbProvider = smartSqlDbProvider,
                    Write = options.DataSource,
                    Read = new List<SmartSql.Configuration.ReadDataSource>()
                },
                SmartSqlMaps = new List<SmartSql.Configuration.SmartSqlMapSource>(),
                TypeHandlers = new List<SmartSql.Configuration.TypeHandler> {
                    new SmartSql.Configuration.TypeHandler
                    {
                          Name="Json", Type="SmartSql.TypeHandler.JsonTypeHandler,SmartSql.TypeHandler"
                    },
                    new SmartSql.Configuration.TypeHandler
                    {
                          Name="PGJson", Type="SmartSql.TypeHandler.PostgreSql.JsonTypeHandler,SmartSql.TypeHandler.PostgreSql"
                    },
                    new SmartSql.Configuration.TypeHandler
                    {
                          Name="PGJsonb", Type="SmartSql.TypeHandler.PostgreSql.JsonbTypeHandler,SmartSql.TypeHandler.PostgreSql"
                    },
                    new SmartSql.Configuration.TypeHandler
                    {
                          Name="OracleBoolean", Type="SmartSql.TypeHandler.Oracle.BooleanTypeHandler,SmartSql.TypeHandler.Oracle"
                    }
                }
            };
            if (!String.IsNullOrEmpty(options.SqlMapPath))
            {
                smartSqlConfigOptions.SmartSqlMaps.Add(new SmartSql.Configuration.SmartSqlMapSource
                {
                    Path = options.SqlMapPath,
                    Type = SmartSql.Configuration.SmartSqlMapSource.ResourceType.Directory
                });
            }
            var _configLoader = new OptionConfigLoader(smartSqlConfigOptions, options.LoggerFactory);
            var smartsqlOptions = new SmartSqlOptions
            {
                Alias = options.Alias,
                ConfigPath = options.Alias,
                ConfigLoader = _configLoader,
                LoggerFactory = options.LoggerFactory
            };
            return MapperContainer.Instance.GetSqlMapper(smartsqlOptions);
        }

        public class CreateSmartSqlMapperOptions
        {
            public string Alias { get; set; } = "SmartSql";
            public string ProviderName { get; set; }
            public SmartSql.Configuration.WriteDataSource DataSource { get; set; }
            public ILoggerFactory LoggerFactory { get; set; }
            public string SqlMapPath { get; set; }
        }
    }
}
