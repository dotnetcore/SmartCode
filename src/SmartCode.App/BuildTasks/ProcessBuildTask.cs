using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using SmartCode.Utilities;

namespace SmartCode.App.BuildTasks
{
    public class ProcessBuildTask : IBuildTask
    {
        const string CREATE_NO_WINDOW = "CreateNoWindow";
        const string WORKING_DIRECTORY = "WorkingDirectory";
        const string WRITE_LINES = "WriteLines";
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
            if (!context.Build.Parameters.Value(FILE_NAME, out string fileName))
            {
                throw new SmartCodeException($"Build:{context.BuildKey},Can not find Parameter:{FILE_NAME}!");
            }
            var startInfo = new ProcessStartInfo(fileName)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = DEFAULT_CREATE_NO_WINDOW
            };
            if (context.Build.Parameters.Value(nameof(ProcessStartInfo.RedirectStandardInput), out bool redirectStandardInput))
            {
                startInfo.RedirectStandardInput = redirectStandardInput;
            }
            if (context.Build.Parameters.Value(nameof(ProcessStartInfo.RedirectStandardOutput), out bool redirectStandardOutput))
            {
                startInfo.RedirectStandardOutput = redirectStandardOutput;
            }
            if (context.Build.Parameters.Value(nameof(ProcessStartInfo.RedirectStandardError), out bool redirectStandardError))
            {
                startInfo.RedirectStandardError = redirectStandardError;
            }
            if (context.Build.Parameters.Value(ARGS, out string args))
            {
                startInfo.Arguments = Handlebars.Compile(args)(context);
            }
            if (context.Build.Parameters.Value(WORKING_DIRECTORY, out string workingDic))
            {
                startInfo.WorkingDirectory = Handlebars.Compile(workingDic)(context);
            }
            if (context.Build.Parameters.Value(CREATE_NO_WINDOW, out bool createNoWin))
            {
                startInfo.CreateNoWindow = createNoWin;
            }
            _logger.LogDebug($"--------Process.FileName:{startInfo.FileName},Args:{startInfo.Arguments} Start--------");
            using (var process = Process.Start(startInfo))
            {
                if (context.Build.Parameters.Value(WRITE_LINES, out IEnumerable<object> lines))
                {
                    foreach (var line in lines)
                    {
                        var lineCommand = Handlebars.Compile(line.ToString())(context);
                        _logger.LogDebug($"StandardInput.WriteLine: [{lineCommand}].");
                        process.StandardInput.WriteLine(lineCommand);
                    }
                }
                if (startInfo.RedirectStandardOutput)
                {
                    var standardOutput = await process.StandardOutput.ReadToEndAsync();
                    if (!String.IsNullOrEmpty(standardOutput))
                    {
                        _logger.LogDebug("StandardOutput start");
                        _logger.LogDebug(standardOutput);
                        _logger.LogDebug("StandardOutput end");
                    }
                }
                if (startInfo.RedirectStandardError)
                {
                    var standardError = await process.StandardError.ReadToEndAsync();
                    if (!String.IsNullOrEmpty(standardError))
                    {
                        _logger.LogDebug("StandardError start");
                        _logger.LogDebug(standardError);
                        _logger.LogDebug("StandardError end");
                    }
                }
                if (!(context.Build.Parameters.Value(nameof(Process.WaitForExit), out bool waitForExit) && !waitForExit))
                {
                    var timeOut = DEFAULT_TIME_OUT;
                    if (context.Build.Parameters.Value(TIMEOUT, out int _timeout))
                    {
                        timeOut = _timeout;
                    }
                    process.WaitForExit(timeOut);
                }
                _logger.LogDebug($"--------Process.FileName:{startInfo.FileName},Args:{startInfo.Arguments},Taken:{process.TotalProcessorTime.TotalMilliseconds} End--------");
            }
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
