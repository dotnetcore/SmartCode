using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.App.BuildTasks
{
    public class ClearBuildTask : IBuildTask
    {
        public ClearBuildTask(ILogger<ClearBuildTask> logger)
        {
            _logger = logger;
        }
        public bool Initialized => true;

        public string Name => "Clear";


        public Task Build(BuildContext context)
        {
            var parameters = context.Build.Parameters;
            if (parameters == null) { return Task.CompletedTask; ; }
            if (parameters.Value("Dirs", out string clearDirs))
            {
                var _clearDirs = clearDirs.Split(',');
                foreach (var dir in _clearDirs)
                {
                    var fullDir = Path.Combine(context.Project.OutputPath, dir);
                    _logger.LogInformation($"ClearBuildTask Delete directory:{fullDir} Start!");
                    if (Directory.Exists(fullDir))
                    {
                        Directory.Delete(fullDir, true);
                    }
                    _logger.LogInformation($"ClearBuildTask Delete directory:{fullDir} End!");
                }
            }
            if (parameters.Value("Files", out string clearFiles))
            {
                var _clearFiles = clearFiles.Split(',');
                foreach (var file in _clearFiles)
                {
                    var fullfile = Path.Combine(context.Project.OutputPath, file);
                    _logger.LogInformation($"ClearBuildTask Delete file:{fullfile} Start!");
                    if (File.Exists(fullfile))
                    {
                        File.Delete(fullfile);
                    }
                    _logger.LogInformation($"ClearBuildTask Delete file:{fullfile} End!");
                }
            }
            return Task.CompletedTask;
        }
        private readonly ILogger<ClearBuildTask> _logger;

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
