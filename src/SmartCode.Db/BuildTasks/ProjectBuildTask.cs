using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db.Entity;
using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Db.BuildTasks
{
    public class ProjectBuildTask : AbstractBuildTask
    {
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<ProjectBuildTask> _logger;

        public ProjectBuildTask(
              IPluginManager pluginManager
            , ILogger<ProjectBuildTask> logger) : base("Project", logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }
        public override async Task Build(BuildContext context)
        {
            var filterTables = FilterTable(context.GetDataSource<DbSource>().Tables, context.BuildKey, context.Build);
            context.SetCurrentAllTable(filterTables);
            context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine).Render(context);
            await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
        }
    }
}
