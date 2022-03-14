using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;

namespace SmartCode.Generator
{
    public class GeneratorProjectBuilder : IProjectBuilder
    {
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<GeneratorProjectBuilder> _logger;

        public GeneratorProjectBuilder(
            Project project
            , IPluginManager pluginManager
            , ILogger<GeneratorProjectBuilder> logger)
        {
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }

        CountdownEvent countdown = new CountdownEvent(1);

        public async Task Build()
        {
            var dataSource = _pluginManager.Resolve<IDataSource>(_project.DataSource.Name);
            await dataSource.InitData();

            //foreach (var buildKV in _project.BuildTasks)
            //{
            //    _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
            //    var output = buildKV.Value.Output;
            //    var buildContext = new BuildContext
            //    {
            //        PluginManager = _pluginManager,
            //        Project = _project,
            //        DataSource = dataSource,
            //        BuildKey = buildKV.Key,
            //        Build = buildKV.Value,
            //        Output = output?.Copy()
            //    };
            //    await _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext);
            //    _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
            //}
            BuildContext[] contexts = _project.BuildTasks.Select(d => new BuildContext
            {
                PluginManager = _pluginManager,
                Project = _project,
                DataSource = dataSource,
                BuildKey = d.Key,
                Build = d.Value,
                Output = d.Value.Output?.Copy(),
            }).ToArray();
            foreach (var context in contexts)
            {
                context.DependOn = contexts.Where(d => d.Build.DependOn.Contains(d.BuildKey)).ToArray();
            }
            countdown.Reset();
            foreach (var context in contexts)
            {
                context.BuildTask = Task.Factory.StartNew(this.BuildTask, null, TaskCreationOptions.LongRunning);
            }

            countdown.Signal();
        }
        private async void BuildTask(object obj)
        {
            countdown.Wait();
            var context = (BuildContext)obj;
            _logger.LogInformation($"-------- BuildTask:{context.BuildKey} Wait! ---------");
            //等待依赖任务
            if (context.DependOn != null)
            {
                foreach (var dcontext in context.DependOn)
                {
                    await dcontext.BuildTask;
                }
            }

            _logger.LogInformation($"-------- BuildTask:{context.BuildKey} Start! ---------");
            //执行自身任务
            await _pluginManager.Resolve<IBuildTask>(context.Build.Type).Build(context);

            _logger.LogInformation($"-------- BuildTask:{context.BuildKey} End! ---------");
        }
    }
}