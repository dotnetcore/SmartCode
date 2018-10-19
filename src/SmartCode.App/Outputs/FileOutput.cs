using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace SmartCode.App.Outputs
{
    public class FileOutput : IOutput
    {
        private readonly ILogger<FileOutput> _logger;

        public FileOutput(ILogger<FileOutput> logger)
        {
            _logger = logger;
        }

        public bool Initialized { get; private set; }

        public string Name { get; private set; } = "File";

        public void Initialize(IDictionary<string, string> paramters)
        {
            Initialized = true;
            if (paramters == null) { return; }
            if (paramters.TryGetValue("Name", out string name))
            {
                Name = name;
            }
        }

        public async Task Output(BuildContext context)
        {
            _logger.LogInformation($"------ Output Build:{context.BuildKey} Start! ------");
            var outputPath = Handlebars.Compile(context.Output.Path)(context);
            outputPath = Path.Combine(context.Project.OutputPath, outputPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                _logger.LogWarning($"------ Output Directory:{outputPath} is not Exists,Created! ------");
            }
            var fileName = Handlebars.Compile(context.Output.Name)(context) + context.Output.Extension;

            var filePath = Path.Combine(outputPath, fileName);
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                await streamWriter.WriteAsync(context.Result);
            }
            _logger.LogInformation($"------ Output Build:{context.BuildKey} -> {filePath} End! ------");
        }
    }
}
