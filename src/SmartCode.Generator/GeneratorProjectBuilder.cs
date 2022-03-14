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
                context.DependOn = contexts.Where(d => context.Build.DependOn != null && context.Build.DependOn.Contains(d.BuildKey)).ToArray();
            }

            countdown.Reset();
            foreach (var context in contexts)
            {
                context.BuildTask = Task.Factory.StartNew(this.BuildTask, context, TaskCreationOptions.LongRunning);
            }

            countdown.Signal();

            await Task.WhenAll(contexts.Select(d => d.BuildTask).ToArray());
        }
        private async void BuildTask(object obj)
        {
            var context = (BuildContext)obj;
            _logger.LogInformation($"-------- BuildTask:{context.BuildKey} Wait! ---------");
            countdown.Wait();
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