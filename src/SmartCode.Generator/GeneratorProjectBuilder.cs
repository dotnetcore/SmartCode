using System.Collections.Generic;
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
        private CountdownEvent countdown = new CountdownEvent(1);

        public GeneratorProjectBuilder(
            Project project
            , IPluginManager pluginManager
            , ILogger<GeneratorProjectBuilder> logger)
        {
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }


        public async Task Build()
        {
            var dataSource = _pluginManager.Resolve<IDataSource>(_project.DataSource.Name);
            await dataSource.InitData();

            this.countdown.Reset();
            foreach (var buildKV in _project.BuildTasks)
            {
                if (!buildKV.Value.WaitPre)
                {
                    this.countdown.AddCount();
                    Thread t = new Thread((obj) =>
                    {
                        var p = ((KeyValuePair<string, Build> buildKV, IDataSource dataSource))obj;
                        BuildTask(buildKV, dataSource);
                        this.countdown.Signal();
                    });

                    t.Start((buildKV, dataSource));
                    continue;
                }

                this.countdown.Signal();
                this.countdown.Wait();
                this.countdown.Reset();

                BuildTask(buildKV, dataSource);
            }
        }

        private void BuildTask(KeyValuePair<string, Build> buildKV, IDataSource dataSource)
        {
            _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
            var output = buildKV.Value.Output;
            BuildContext buildContext = new BuildContext
            {
                PluginManager = _pluginManager,
                Project = _project,
                DataSource = dataSource,
                BuildKey = buildKV.Key,
                Build = buildKV.Value,
                Output = output?.Copy()
            };
            _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext).Wait();
            _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
        }
    }
}