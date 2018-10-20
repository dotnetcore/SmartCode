using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.TemplateEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.App.BuildTasks
{
    /// <summary>
    /// 多模板->多单文件构建任务
    /// </summary>
    public class MultiTemplateBuildTask : IBuildTask
    {
        const String TEMPLATES_KEY = "Templates";
        const String TEMPLATE_KEY = "Key";
        const String TEMPLATE_OUTPUT_KEY = "Output";
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<MultiTemplateBuildTask> _logger;

        public bool Initialized => true;

        public string Name => "MultiTemplate";

        public MultiTemplateBuildTask(IPluginManager pluginManager
            , ILogger<MultiTemplateBuildTask> logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }
        public async Task Build(BuildContext context)
        {
            if (context.Build.Paramters.TryGetValue(TEMPLATES_KEY, out object templates))
            {
                var _templates = templates as IEnumerable;
                foreach (var templateKVs in _templates)
                {
                    var _templateKVs = (Dictionary<object, object>)templateKVs;
                    if (!_templateKVs.TryGetValue(TEMPLATE_KEY, out object templateKey))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Can not find TemplateKey!");
                    }
                    context.Build.Template = templateKey.ToString();
                    context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine).Render(context);
                    if (!_templateKVs.TryGetValue(TEMPLATE_OUTPUT_KEY, out object output))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Can not find Output!");
                    }
                    if (context.Output == null)
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Output can not be null!");
                    }
                    var outputKVs = (Dictionary<object, object>)output;
                    if (outputKVs.TryGetValue(nameof(Output.Path), out object outputPath))
                    {
                        context.Output.Path = outputPath.ToString();
                    }
                    if (outputKVs.TryGetValue(nameof(Output.Mode), out object outputMode))
                    {
                        context.Output.Mode = (CreateMode)Enum.Parse(typeof(CreateMode), outputMode.ToString());
                    }
                    if (String.IsNullOrEmpty(context.Output.Path))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Template:{templateKey},can not find Output.Path!");
                    }
                    if (!outputKVs.TryGetValue(nameof(Output.Name), out object outputName))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Template:{templateKey},can not find Output.Name!");
                    }
                    context.Output.Name = outputName.ToString();

                    if (!outputKVs.TryGetValue(nameof(Output.Extension), out object extension))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Template:{templateKey},can not find Output.Extension!");
                    }
                    context.Output.Extension = extension.ToString();

                    await _pluginManager.Resolve<IOutput>(context.Output.Type).Output(context);
                }
            }
        }

        public void Initialize(IDictionary<string, string> paramters)
        {

        }
    }
}
