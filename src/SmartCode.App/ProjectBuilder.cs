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
        public async Task Build()
        {
            foreach (var buildKV in _project.BuildTasks)
            {
                _logger.LogInformation($"-------- BuildTask:{buildKV.Key} Start! ---------");
                BuildContext buildContext = new BuildContext
                {
                    Project = _project,
                    DataSource = _pluginManager.Resolve<IDataSource>(_project.DataSource.Name),
                    BuildKey = buildKV.Key,
                    Build = buildKV.Value
                };
                await _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext);
                _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
            }
        }

    }
}
