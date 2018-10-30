using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.ETL.BuildTasks
{
    public class TransformBuildTask : IBuildTask
    {
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
            await _pluginManager.Resolve<ITransformEngine>(DEFAULT_SCRIPT_ENGINE).Transform(context);
        }

        public void Initialize(IDictionary<string, object> paramters)
        {

        }
    }
}
