using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;
using SmartSql;

namespace SmartCode.Db
{
    public class DbTableRepository : DbRepository
    {
        private ILogger<DbTableRepository> _logger;
        public String Scope { get; set; }

        public DbTableRepository(
            DataSource dataSource
            , ILoggerFactory loggerFactory) : base(dataSource, loggerFactory)
        {
            Scope = $"Database-{DbProviderName}";
            _logger = loggerFactory.CreateLogger<DbTableRepository>();
            switch (DbProvider)
            {
                case DbProvider.PostgreSql:
                    {
                        if (String.IsNullOrEmpty(DbSchema))
                        {
                            DbSchema = "public";
                        }
                        break;
                    }
            }
        }
        public async Task<IList<Table>> QueryTable()
        {
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName}, QueryTable Start! ----");
            IList<Table> tables;
            try
            {
                SqlMapper.SessionStore.Open();
                tables = await SqlMapper.QueryAsync<Table>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "QueryTable",
                    Request = new { DbName, DbSchema }
                });
                foreach (var table in tables)
                {
                    table.Columns = await SqlMapper.QueryAsync<Column>(new RequestContext
                    {
                        Scope = Scope,
                        SqlId = "QueryColumn",
                        Request = new { DbName, DbSchema, TableId = table.Id, TableName = table.Name }
                    });
                }
            }
            finally
            {
                SqlMapper.SessionStore.Dispose();
            }
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName},Tables:{tables.Count()} QueryTable End! ----");
            return tables;
        }
    }
}
