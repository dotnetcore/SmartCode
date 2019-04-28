using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Utilities;

namespace SmartCode.TemplateEngine.Impl
{
    public class HandlebarsTemplateEngine : ITemplateEngine
    {
        private readonly Project _project;
        private readonly ILogger<HandlebarsTemplateEngine> _logger;
        private IDictionary<String, Func<Object, string>> _cahcedTemplate = new Dictionary<String, Func<Object, String>>();
        public bool Initialized { get; private set; }
        public string Name { get; private set; } = "Handlebars";
        private string _root = AppPath.Relative("HandlebarsTemplates");
        public HandlebarsTemplateEngine(Project project
            , ILogger<HandlebarsTemplateEngine> logger
            )
        {
            _project = project;
            _logger = logger;
        }

        private Func<object, string> CompileTemplate(string templatePath)
        {
            var templateFullPath = Path.Combine(_root, templatePath);
            _logger.LogInformation($"------ HandlebarsTemplateEngine CompileTemplate:{templateFullPath} Start! ------");
            using (var streamReader = File.OpenText(templateFullPath))
            {
                var templateContent = streamReader.ReadToEnd();
                var template = Handlebars.Compile(templateContent);
                _logger.LogInformation($"------ HandlebarsTemplateEngine CompileTemplate:{templateFullPath} End! ------");
                return template;
            }
        }
        private Func<object, string> GetTemplate(string templatePath)
        {
            if (!_cahcedTemplate.ContainsKey(templatePath))
            {
                _cahcedTemplate[templatePath] = CompileTemplate(templatePath);
            }
            return _cahcedTemplate[templatePath];
        }

        public Task<string> Render(BuildContext context)
        {
            _logger.LogDebug($"------ Render Build:{context.BuildKey} Start! ------");
            var template = GetTemplate(context.Build.TemplateEngine.FullPath);
            context.Result = template(context);
            _logger.LogDebug($"------ Render Build:{context.BuildKey} End! ------");
            return Task.FromResult(context.Result);
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters == null) { return; }
            if (parameters.Value("Name", out string name))
            {
                Name = name;
            }
            if (parameters.Value("Root", out string root))
            {
                _root = root;
            }
            Initialized = true;
        }

    }
}
