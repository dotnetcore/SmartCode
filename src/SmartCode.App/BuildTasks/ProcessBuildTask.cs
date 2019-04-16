using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace SmartCode.App.BuildTasks
{
    public class ProcessBuildTask : IBuildTask
    {
        const string CREATE_NO_WINDOW = "CreateNoWindow";
        const string WORKING_DIRECTORY = "WorkingDirectory";
        const string FILE_NAME = "FileName";
        const string ARGS = "Args";
        const string TIMEOUT = "Timeout";
        private readonly ILogger<ProcessBuildTask> _logger;
        const int DEFAULT_TIME_OUT = 30 * 1000;
        const bool DEFAULT_CREATE_NO_WINDOW = true;
        public bool Initialized => true;

        public string Name => "Process";
        public ProcessBuildTask(ILogger<ProcessBuildTask> logger)
        {
            _logger = logger;
        }

        public async Task Build(BuildContext context)
        {
            if (!context.Build.Paramters.Value(FILE_NAME, out string fileName))
            {
                throw new SmartCodeException($"Build:{context.BuildKey},Can not find Parameter:{FILE_NAME}!");
            }
            if (!context.Build.Paramters.Value(ARGS, out string args))
            {
                throw new SmartCodeException($"Build:{context.BuildKey},Can not find Parameter:{ARGS}!");
            }
            var process = new Process();
            var startInfo = process.StartInfo;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = DEFAULT_CREATE_NO_WINDOW;
            startInfo.FileName = fileName;
            startInfo.Arguments = Handlebars.Compile(args)(context);
            if (context.Build.Paramters.Value(WORKING_DIRECTORY, out string workingDic))
            {
                startInfo.WorkingDirectory = Handlebars.Compile(workingDic)(context);
            }
            if (context.Build.Paramters.Value(CREATE_NO_WINDOW, out bool createNoWin))
            {
                startInfo.CreateNoWindow = createNoWin;
            }
            _logger.LogDebug($"--------Process.FileName:{startInfo.FileName},Args:{startInfo.Arguments} Start--------");
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
            try
            {
                process.Start();
                var timeOut = DEFAULT_TIME_OUT;
                if (context.Build.Paramters.Value(TIMEOUT, out int _timeout))
                {
                    timeOut = _timeout;
                }
                process.WaitForExit(timeOut);

                var standardOutput = await process.StandardOutput.ReadToEndAsync();
                if (!String.IsNullOrEmpty(standardOutput))
                {
                    _logger.LogDebug("StandardOutput start");
                    _logger.LogDebug(standardOutput);
                    _logger.LogDebug("StandardOutput end");
                }
                var standardError = await process.StandardError.ReadToEndAsync();
                if (!String.IsNullOrEmpty(standardError))
                {
                    _logger.LogDebug("StandardError start");
                    _logger.LogDebug(standardError);
                    _logger.LogDebug("StandardError end");
                }
                _logger.LogDebug($"--------Process.FileName:{startInfo.FileName},Args:{startInfo.Arguments},Taken:{process.TotalProcessorTime.TotalMilliseconds} End--------");
            }
            finally
            {
                process.Dispose();
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogDebug(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogDebug(e.Data);
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
