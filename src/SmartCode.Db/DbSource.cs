using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
        public SmartSql.SmartSqlOptions SmartSqlOptions { get { return DbRepository.SqlMapper.SmartSqlOptions; } }
        public SmartSql.SmartSqlContext SmartSqlContext { get { return SmartSqlOptions.SmartSqlContext; } }
        public SmartSql.Configuration.Database Database { get { return SmartSqlContext.Database; } }
        public SmartSql.Configuration.DbProvider DbProvider { get { return Database.DbProvider; } }
        public SmartSql.Configuration.WriteDataSource WriteDataSource { get { return Database.WriteDataSource; } }

        public Project Project { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IPluginManager PluginManager { get; }

        public void Initialize(IDictionary<string, object> paramters)
        {
            if (paramters != null)
            {
                if (paramters.Value("Name", out string name))
                {
                    Name = name;
                }
            }

            this.Initialized = true;
        }

        public virtual void InitData()
        {
            DbRepository = new DbRepository(Project.DataSource, LoggerFactory);
        }
    }
}
