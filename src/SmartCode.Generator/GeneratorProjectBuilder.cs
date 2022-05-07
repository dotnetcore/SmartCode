using System;
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

            if (_project.AllowParallel)
            {
                await this.ParallelBuild(dataSource);
            }
            else
            {
                await this.SerialBuild(dataSource);
            }
        }

        public async Task SerialBuild(IDataSource dataSource)
        {
            foreach (var buildKV in _project.BuildTasks)
            {
                _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
                var context = new BuildContext
                {
                    PluginManager = _pluginManager,
                    Project = _project,
                    DataSource = dataSource,
                    BuildKey = buildKV.Key,
                    Build = buildKV.Value,
                    Output = buildKV.Value.Output?.Copy(),
                };

                //执行自身任务
                await _pluginManager.Resolve<IBuildTask>(context.Build.Type).Build(context);

                _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
            }
        }

        private Task ParallelBuild(IDataSource dataSource)
        {

            IList<BuildContext> allContexts = _project.BuildTasks.Select(d => new BuildContext
            {
                PluginManager = _pluginManager,
                Project = _project,
                DataSource = dataSource,
                BuildKey = d.Key,
                Build = d.Value,
                Output = d.Value.Output?.Copy(),
            }).ToArray();
            foreach (var context in allContexts)
            {
                if (context.Build.DependOn != null && context.Build.DependOn.Count() > 0)
                {
                    context.DependOn = allContexts.Where(d => context.Build.DependOn.Contains(d.BuildKey)).ToArray();
                }
            }
            countdown.Reset();
            countdown.AddCount(allContexts.Count);
            foreach (var context in allContexts)
            {
                context.CountDown.Reset();
                if (context.DependOn != null && context.DependOn.Count > 0)
                {
                    context.CountDown.AddCount(context.DependOn.Count);
                }

                ThreadPool.QueueUserWorkItem((obj) => _ = this.BuildTask(obj), (context, allContexts));
            }

            foreach (var context in allContexts)
            {
                context.CountDown.Signal();
            }

            countdown.Signal();
            countdown.Wait();

            return Task.CompletedTask;
        }

        private async Task BuildTask(object obj)
        {
            var p = ((BuildContext context, IList<BuildContext> allContexts))obj;

            if (p.context.DependOn != null && p.context.DependOn.Count > 0)
            {
                _logger.LogInformation($"-------- BuildTask:{p.context.BuildKey} Wait [{string.Join(",", p.context.DependOn?.Select(d => d.BuildKey)?.ToArray())}]---------");
            }
            //等待依赖任务
            p.context.CountDown.Wait();

            _logger.LogInformation($"-------- BuildTask:{p.context.BuildKey} Start! ---------");
            //执行自身任务
            await _pluginManager.Resolve<IBuildTask>(p.context.Build.Type).Build(p.context);

            foreach (var c in p.allContexts)
            {
                if (c.DependOn == null || c.DependOn.Count == 0)
                {
                    continue;
                }
                if (c.DependOn.Contains(p.context))
                {
                    c.CountDown.Signal();
                }
            }

            countdown.Signal();
            _logger.LogInformation($"-------- BuildTask:{p.context.BuildKey} End! ---------");
        }
    }
}