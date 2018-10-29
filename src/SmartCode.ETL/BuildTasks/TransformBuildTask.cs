using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.ETL.BuildTasks
{
    public class TransformBuildTask : IBuildTask
    {
        private const String SCRIPT = "Script";
        private const String SCRIPT_ENGINE = "Engine";
        private const String DEFAULT_SCRIPT_ENGINE = "Razor";
        private readonly IPluginManager _pluginManager;

        public bool Initialized => true;

        public string Name => "Transform";
        public TransformBuildTask(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }
        public async Task Build(BuildContext context)
        {
            var dataSource = context.GetDataSource<ExtractDataSource>();
            dataSource.TransformData = dataSource.Data;
            if (context.Build.Paramters.Value(SCRIPT, out string script) && !String.IsNullOrEmpty(script))
            {
                context.Build.Template = script;
                await _pluginManager.Resolve<ITemplateEngine>(DEFAULT_SCRIPT_ENGINE).Render(context);
            }
        }

        public void Initialize(IDictionary<string, object> paramters)
        {

        }
    }
}
