using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SmartCode.App
{
    public class ProjectBuilder : IProjectBuilder
    {
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<ProjectBuilder> _logger;

        public ProjectBuilder(
            Project project
            , IPluginManager pluginManager
            , ILogger<ProjectBuilder> logger)
        {
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public event OnProjectBuildStartupHandler OnStartup;
        public event OnProjectBuildSucceedHandler OnSucceed;
        public event OnProjectBuildFailedHandler OnFailed;


        public async Task Build()
        {
            BuildContext buildContext = null;
            try
            {
                var dataSource = _pluginManager.Resolve<IDataSource>(_project.DataSource.Name);
                if (OnStartup != null)
                    await OnStartup.Invoke(this, new OnProjectBuildStartupEventArgs {Project = _project});
                await dataSource.InitData();
                foreach (var buildKV in _project.BuildTasks)
                {
                    _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
                    var output = buildKV.Value.Output;
                    buildContext = new BuildContext
                    {
                        PluginManager = _pluginManager,
                        Project = _project,
                        DataSource = dataSource,
                        BuildKey = buildKV.Key,
                        Build = buildKV.Value,
                        Output = output?.Copy()
                    };
                    await _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext);
                    _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
                }

                if (OnSucceed != null)
                    await OnSucceed.Invoke(this, new OnProjectBuildSucceedEventArgs
                    {
                        Project = _project
                    });
            }
            catch (Exception ex)
            {
                if (OnFailed != null)
                    await OnFailed.Invoke(this, new OnProjectBuildFailedEventArgs
                    {
                        Project = _project,
                        Context = buildContext,
                        ErrorException = ex
                    });
                throw;
            }
        }
    }
}