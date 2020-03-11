using SmartCode.Configuration;
using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.ETL.BuildTasks
{
    public class TransformBuildTask : IBuildTask
    {
        private const String DEFAULT_SCRIPT_ENGINE = "Razor";
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;

        public bool Initialized => true;

        public string Name => "Transform";
        public TransformBuildTask(
            Project project
            , IPluginManager pluginManager)
        {
            _project = project;
            _pluginManager = pluginManager;
        }
        public async Task Build(BuildContext context)
        {
            var etlRepository = _pluginManager.Resolve<IETLTaskRepository>(_project.GetETLRepository());
            Stopwatch stopwatch = Stopwatch.StartNew();
            var dataSource = context.GetExtractData<AbstractExtractData>();
            if (dataSource.GetCount()> 0)
            {
                await _pluginManager.Resolve<ITransformEngine>(DEFAULT_SCRIPT_ENGINE).Transform(context);
            }
            stopwatch.Stop();
            await etlRepository.Transform(_project.GetETKTaskId(), new Entity.ETLTransform
            {
                Taken = stopwatch.ElapsedMilliseconds
            });
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
