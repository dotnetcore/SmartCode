using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSql.Configuration;
using SmartSql.DataSource;

namespace SmartCode.Db
{
    public class DbSource : IDataSource
    {
        public DbSource(
           Project project
            , ILoggerFactory loggerFactory
            , IPluginManager pluginManager
            )
        {
            Project = project;
            LoggerFactory = loggerFactory;
            PluginManager = pluginManager;
        }

        public bool Initialized { get; private set; }

        public virtual string Name { get; private set; } = "Db";
        public DbRepository DbRepository { get; protected set; }
        public SmartSqlConfig SmartSqlConfig => DbRepository.SqlMapper.SmartSqlConfig;
        public Database Database => SmartSqlConfig.Database;
        public SmartSql.DataSource.DbProvider DbProvider => Database.DbProvider;
        public WriteDataSource WriteDataSource => Database.Write;

        public Project Project { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IPluginManager PluginManager { get; }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                if (parameters.Value("Name", out string name))
                {
                    Name = name;
                }
            }

            this.Initialized = true;
        }

        public virtual Task InitData()
        {
            DbRepository = new DbRepository(Project.DataSource, LoggerFactory);
            return Task.CompletedTask;
        }
    }
}
