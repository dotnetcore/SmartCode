using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.App.BuildTasks
{
    public class ProcessBuildTask : IBuildTask
    {
        const string WORKING_DIRECTORY = "WorkingDirectory";
        const string FILE_NAME = "FileName";
        const string ARGS = "Args";
        const string TIMEOUT = "Timeout";
        private readonly ILogger<ProcessBuildTask> _logger;
        const int DEFAULT_TIME_OUT = 30 * 1000;
        public bool Initialized => true;

        public string Name => "Process";
        public ProcessBuildTask(ILogger<ProcessBuildTask> logger)
        {
            _logger = logger;
        }

        public Task Build(BuildContext context)
        {
            if (!context.Build.Paramters.TryGetValue(FILE_NAME, out object fileNameObj))
            {
                throw new SmartCodeException($"Build:{context.BuildKey},Can not find Paramter:{FILE_NAME}!");
            }
            var fileName = fileNameObj.ToString();
            if (!context.Build.Paramters.TryGetValue(ARGS, out object argsObj))
            {
                throw new SmartCodeException($"Build:{context.BuildKey},Can not find Paramter:{ARGS}!");
            }
            var args = argsObj.ToString();
            var startInfo = new ProcessStartInfo(fileName, args);

            var timeOut = DEFAULT_TIME_OUT;
            if (context.Build.Paramters.TryGetValue(TIMEOUT, out object timeoutObj))
            {
                if (int.TryParse(timeoutObj.ToString(), out int _timeout))
                {
                    timeOut = _timeout;
                }
            }
            if (context.Build.Paramters.TryGetValue(WORKING_DIRECTORY, out object workingDicObj))
            {
                startInfo.WorkingDirectory = workingDicObj.ToString();
            }
            _logger.LogDebug($"--------Process.FileName:{fileName},Args:{args} Start--------");
            var process = Process.Start(startInfo);
            try
            {
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.WaitForExit(timeOut);
                _logger.LogDebug($"--------Process.FileName:{fileName},Args:{args} End--------");
            }
            finally
            {
                process.Dispose();
            }
            
            return Task.CompletedTask;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogDebug(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogDebug(e.Data);
        }

        public void Initialize(IDictionary<string, string> paramters)
        {

        }
    }
}
