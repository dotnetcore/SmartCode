using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;
using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCode.Generator.BuildTasks
{
    public class TableBuildTask : AbstractDbBuildTask
    {
        public const String IGNORENOPKTABLE_KEY = nameof(IgnoreNoPKTable);
        public bool IgnoreNoPKTable { get; set; }
        private readonly ILogger<TableBuildTask> _logger;
        private readonly IPluginManager _pluginManager;

        public TableBuildTask(IPluginManager pluginManager
            , ILogger<TableBuildTask> logger) : base("Table", logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public override async Task Build(BuildContext context)
        {
            var filterTables = FilterTable(context.GetDataSource<ITableSource>().Tables, context.BuildKey,
                context.Build);
            context.SetCurrentAllTable(filterTables);
            foreach (var table in filterTables)
            {
                _logger.LogInformation($"BuildTable:{table.Name} Start!");
                context.SetCurrentTable(table);
                _pluginManager.Resolve<INamingConverter>().Convert(context);
                context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name)
                    .Render(context);
                await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
                _logger.LogInformation($"BuildTable:{table.Name} End!");
            }
        }

        public override void Initialize(IDictionary<string, object> parameters)
        {
            base.Initialize(parameters);
        }
    }
}