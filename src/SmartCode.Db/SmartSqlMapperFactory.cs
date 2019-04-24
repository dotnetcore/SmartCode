using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using SmartSql.ConfigBuilder;

namespace SmartCode.Db
{
    public class SmartSqlMapperFactory
    {
        public static ISqlMapper Create(CreateSmartSqlMapperOptions options)
        {
            SmartSqlConfigOptions smartSqlConfigOptions = new SmartSqlConfigOptions
            {
                Settings = new SmartSql.Configuration.Settings
                {
                    ParameterPrefix = "$",
                    IgnoreParameterCase = true,
                    IsCacheEnabled = false,
                },
                Database = new Database
                {
                    DbProvider = new SmartSql.DataSource.DbProvider { Name = options.ProviderName },
                    Write = options.DataSource,
                    Reads = new List<DataSource>()
                },
                SmartSqlMaps = new List<SqlMapSource>(),
                TypeHandlers = new List<TypeHandler>
                {
                    new TypeHandler
                    {
                        Name = "Json", Type = "SmartSql.TypeHandler.JsonTypeHandler,SmartSql.TypeHandler"
                    },
                    new TypeHandler
                    {
                        Name = "PGJson",
                        Type = "SmartSql.TypeHandler.PostgreSql.JsonTypeHandler,SmartSql.TypeHandler.PostgreSql"
                    },
                    new TypeHandler
                    {
                        Name = "PGJsonb",
                        Type = "SmartSql.TypeHandler.PostgreSql.JsonTypeHandler,SmartSql.TypeHandler.PostgreSql",
                        Properties=new Dictionary<string, object>
                        {
                            { "DataTypeName","jsonb"}
                        }
                    }
                }
            };
            if (!String.IsNullOrEmpty(options.SqlMapPath))
            {
                smartSqlConfigOptions.SmartSqlMaps.Add(new SqlMapSource
                {
                    Path = options.SqlMapPath,
                    Type = ResourceType.Directory
                });
            }

            var optionConfigBuilder = new OptionConfigBuilder(smartSqlConfigOptions, options.LoggerFactory);
            return new SmartSqlBuilder()
                   .UseLoggerFactory(options.LoggerFactory)
                   .UseConfigBuilder(optionConfigBuilder)
                   .UseAlias(options.Alias)
                   .Build().SqlMapper;
        }

        public class CreateSmartSqlMapperOptions
        {
            public string Alias { get; set; } = "SmartSql";
            public string ProviderName { get; set; }
            public DataSource DataSource { get; set; }
            public ILoggerFactory LoggerFactory { get; set; }
            public string SqlMapPath { get; set; }
        }
    }
}
