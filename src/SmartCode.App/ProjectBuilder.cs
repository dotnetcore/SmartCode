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
            var dataSource = _pluginManager.Resolve<IDataSource>(_project.DataSource.Name);
            dataSource.InitData();
            foreach (var buildKV in _project.BuildTasks)
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
                    Output = output == null ? null : new Output
                    {
                        Type = output.Type,
                        Path = output.Path,
                        Name = output.Name,
                        Mode = output.Mode,
                        Extension = output.Extension
                    }
                };
                await _pluginManager.Resolve<IBuildTask>(buildKV.Value.Type).Build(buildContext);
                _logger.LogInformation($"-------- BuildTask:{buildKV.Key} End! ---------");
            }
        }

    }
}
