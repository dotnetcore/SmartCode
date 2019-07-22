using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db;
using SmartCode.Generator.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Generator
{
    public class DbTableSource : DbSource, ITableSource
    {
        public DbTableSource(
            Project project
            , ILoggerFactory loggerFactory
            , IPluginManager pluginManager
        ) : base(project, loggerFactory, pluginManager)
        {
        }

        public override string Name => "DbTable";
        public IList<Table> Tables { get; private set; }

        public override async Task InitData()
        {
            var dbTableRepository = new DbTableRepository(Project.DataSource, LoggerFactory);
            DbRepository = dbTableRepository;
            Tables = await dbTableRepository.QueryTable();
            var dbTypeConvert = PluginManager.Resolve<IDbTypeConverter>();
            foreach (var table in Tables)
            {
                foreach (var col in table.Columns)
                {
                    if ((DbRepository.DbProvider == Db.DbProvider.MySql ||
                         DbRepository.DbProvider == Db.DbProvider.MariaDB)
                        && col.DbType == "char"
                        && col.DataLength == 36
                        && Project.Language == "CSharp")
                    {
                        col.LanguageType = "Guid";
                    }
                    else
                    {
                        col.LanguageType =
                            dbTypeConvert.LanguageType(DbRepository.DbProvider, Project.Language, col.DbType);
                    }
                }
            }
        }
    }
}