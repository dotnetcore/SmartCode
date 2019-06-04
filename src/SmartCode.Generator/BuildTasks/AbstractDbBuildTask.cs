using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Generator.BuildTasks
{
    public abstract class AbstractDbBuildTask : IBuildTask
    {
        private readonly ILogger _logger;

        public AbstractDbBuildTask(string name, ILogger logger)
        {
            Name = name;
            _logger = logger;
        }

        public bool Initialized { get; protected set; }

        public string Name { get; protected set; }

        public abstract Task Build(BuildContext context);

        public virtual void Initialize(IDictionary<string, object> parameters)
        {
            this.Initialized = true;
        }

        protected IList<Table> FilterTable(IEnumerable<Table> tables, string buildKey, Build build)
        {
            _logger.LogInformation($"FilterTable Build:{buildKey} Start!");
            IEnumerable<Table> buildTables = CopyTables(tables);
            if (build.IgnoreNoPKTable.HasValue && build.IgnoreNoPKTable.Value)
            {
                _logger.LogInformation($"FilterTable Build:{buildKey} IgnoreNoPKTable!");
                buildTables = buildTables.Where(m => m.PKColumn != null);
            }

            if (build.IgnoreView.HasValue && build.IgnoreView.Value)
            {
                _logger.LogInformation($"FilterTable Build:{buildKey} IgnoreView!");
                buildTables = buildTables.Where(m => m.Type != Table.TableType.View);
            }

            if (build.IgnoreTables != null)
            {
                _logger.LogInformation(
                    $"FilterTable Build:{buildKey} IgnoreTables: [{String.Join(",", build.IgnoreTables)}]!");
                buildTables = buildTables.Where(m => !build.IgnoreTables.Contains(m.Name));
            }

            if (build.IncludeTables != null)
            {
                _logger.LogInformation(
                    $"FilterTable Build:{buildKey} IncludeTables: [{String.Join(",", build.IncludeTables)}]!");
                buildTables = buildTables.Where(m => build.IncludeTables.Contains(m.Name));
            }

            _logger.LogInformation($"FilterTable Build:{buildKey} End!");
            return buildTables.ToList();
        }

        protected IList<Table> CopyTables(IEnumerable<Table> tables)
        {
            return tables.Select(m => new Table
            {
                Id = m.Id,
                Name = m.Name,
                TypeName = m.TypeName,
                ConvertedName = m.ConvertedName,
                Description = m.Description,
                Columns = m.Columns.Select(c => new Column
                {
                    Id = c.Id,
                    Name = c.Name,
                    DbType = c.DbType,
                    Description = c.Description,
                    AutoIncrement = c.AutoIncrement,
                    ConvertedName = c.ConvertedName,
                    IsNullable = c.IsNullable,
                    IsPrimaryKey = c.IsPrimaryKey,
                    LanguageType = c.LanguageType,
                    DataLength = c.DataLength
                }).ToList()
            }).ToList();
        }
    }
}