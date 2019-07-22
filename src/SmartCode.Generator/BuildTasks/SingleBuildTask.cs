using Microsoft.Extensions.Logging;
using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Generator.BuildTasks
{
    public class SingleBuildTask : AbstractDbBuildTask
    {
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<SingleBuildTask> _logger;

        public SingleBuildTask(IPluginManager pluginManager
        , ILogger<SingleBuildTask> logger) : base("Single", logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }
        public override async Task Build(BuildContext context)
        {
            
            var filterTables = FilterTable(context.GetDataSource<ITableSource>().Tables, context.BuildKey, context.Build);
            context.SetCurrentAllTable(filterTables);
            foreach (var table in filterTables)
            {
                context.SetCurrentTable(table);
                _pluginManager.Resolve<INamingConverter>().Convert(context);
            }
            context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name).Render(context);
            await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
        }
    }
}
