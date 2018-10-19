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
        public IEnumerable<Table> Tables { get; private set; }

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

        public void InitData()
        {
            DbRepository = new DbRepository(_project, _loggerFactory);
            Tables = DbRepository.QueryTable();
            var dbTypeConvert = _pluginManager.Resolve<IDbTypeConverter>();
            foreach (var table in Tables)
            {
                foreach (var col in table.Columns)
                {
                    if ((DbRepository.DbProvider == Db.DbProvider.MySql || DbRepository.DbProvider == Db.DbProvider.MariaDB)
                        && col.DbType == "char"
                        && col.DataLength == 36
                        && _project.Language == "CSharp")
                    {
                        col.LanguageType = "Guid";
                    }
                    else
                    {
                        col.LanguageType = dbTypeConvert.LanguageType(DbRepository.DbProvider, _project.Language, col.DbType);
                    }
                }
            }
        }
    }
}
