using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RazorLight;
using SmartCode.Configuration;
using SmartCode.Utilities;

namespace SmartCode.TemplateEngine.Impl
{
    public class RazorTemplateEngine : ITemplateEngine
    {
        private readonly Project _project;
        private readonly ILogger<RazorTemplateEngine> _logger;
        private IRazorLightEngine _razorLightEngine;
        public string Name { get; private set; } = "Razor";
        private string _root = AppPath.Relative("RazorTemplates");
        public RazorTemplateEngine(
            Project project
            , ILogger<RazorTemplateEngine> logger)
        {
            _project = project;
            _logger = logger;
        }
        public bool Initialized { get; private set; }
        public async Task<String> Render(BuildContext context)
        {
            _logger.LogDebug($"------ Render Build:{context.BuildKey} Start! ------");
            context.Result = await _razorLightEngine.CompileRenderAsync<BuildContext>(context.Build.Template, context);
            _logger.LogDebug($"------ Render Build:{context.BuildKey} End! ------");
            return context.Result;
        }
        public void Initialize(IDictionary<string, String> paramters)
        {
            Initialized = true;
            if (paramters != null)
            {
                if (paramters.TryGetValue("Name", out string name))
                {
                    Name = name;
                }
                if (paramters.TryGetValue("Root", out string root))
                {
                    _root = root;
                }
            }
            _razorLightEngine = new RazorLightEngineBuilder()
                .UseFilesystemProject(_root)
                .UseMemoryCachingProvider()
                .Build();
        }
    }
}
