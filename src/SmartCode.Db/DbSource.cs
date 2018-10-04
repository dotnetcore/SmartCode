using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Db
{
    public class DbSource : IDataSource
    {
        private readonly Project _project;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPluginManager _pluginManager;

        public DbSource(
            Project project
            , ILoggerFactory loggerFactory
            , IPluginManager pluginManager
            )
        {
            _project = project;
            _loggerFactory = loggerFactory;
            _pluginManager = pluginManager;
        }

        public bool Initialized { get; private set; }

        public string Name { get; private set; } = "Db";
        public DbRepository DbRepository { get; private set; }
        public SmartSql.SmartSqlOptions SmartSqlOptions { get { return DbRepository.SqlMapper.SmartSqlOptions; } }
        public SmartSql.SmartSqlContext SmartSqlContext { get { return SmartSqlOptions.SmartSqlContext; } }
        public SmartSql.Configuration.Database Database { get { return SmartSqlContext.Database; } }
        public SmartSql.Configuration.DbProvider DbProvider { get { return Database.DbProvider; } }
        public SmartSql.Configuration.WriteDataSource WriteDataSource { get { return Database.WriteDataSource; } }
        private IEnumerable<Table> _tables;
        public IEnumerable<Table> Tables
        {
            get
            {
                if (_tables == null)
                {
                    DbRepository = new DbRepository(_project, _loggerFactory);
                    _tables = DbRepository.QueryTable();
                    var dbTypeConvert = _pluginManager.Resolve<IDbTypeConverter>();
                    foreach (var table in Tables)
                    {
                        foreach (var col in table.Columns)
                        {
                            dbTypeConvert.LanguageType(DbRepository.DbProvider, _project.Language, col.DbType);
                        }
                    }
                }
                return _tables;
            }
        }

        public void Initialize(IDictionary<string, String> paramters)
        {
            if (paramters != null)
            {
                if (paramters.TryGetValue("Name", out string name))
                {
                    Name = name;
                }
            }

            this.Initialized = true;
        }
    }
}
