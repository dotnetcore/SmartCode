using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.TemplateEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
            if (context.Build.Parameters.Value(TEMPLATES_KEY, out IEnumerable templates))
            {
                foreach (var templateKVs in templates)
                {
                    var _templateKVs = (Dictionary<object, object>) templateKVs;
                    if (!_templateKVs.Value(TEMPLATE_KEY, out string templateKey))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Can not find TemplateKey!");
                    }

                    context.Build.TemplateEngine.Path = templateKey;
                    context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name)
                        .Render(context);
                    if (!_templateKVs.Value(TEMPLATE_OUTPUT_KEY, out Dictionary<object, object> outputKVs))
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Can not find Output!");
                    }

                    if (context.Output == null)
                    {
                        throw new SmartCodeException($"Build:{context.BuildKey},Output can not be null!");
                    }

                    Output output = new Output
                    {
                        Path = context.Output.Path,
                        Mode = context.Output.Mode,
                        DotSplit = context.Output.DotSplit,
                        Name = context.Output.Name,
                        Extension = context.Output.Extension
                    };
                    if (outputKVs.Value(nameof(Output.Path), out string outputPath))
                    {
                        output.Path = outputPath;
                    }

                    if (outputKVs.Value(nameof(Output.Mode), out CreateMode outputMode))
                    {
                        output.Mode = outputMode;
                    }

                    if (outputKVs.Value(nameof(Output.DotSplit), out string dotSplit))
                    {
                        output.DotSplit = Convert.ToBoolean(dotSplit);
                    }

                    if (String.IsNullOrEmpty(output.Path))
                    {
                        throw new SmartCodeException(
                            $"Build:{context.BuildKey},Template:{templateKey},can not find Output.Path!");
                    }

                    if (!outputKVs.Value(nameof(Output.Name), out string outputName))
                    {
                        throw new SmartCodeException(
                            $"Build:{context.BuildKey},Template:{templateKey},can not find Output.Name!");
                    }

                    output.Name = outputName;

                    if (outputKVs.Value(nameof(Output.Extension), out string extension))
                    {
                        output.Extension = extension;
                    }

                    await _pluginManager.Resolve<IOutput>(context.Output.Type).Output(context, output);
                }
            }
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
        }
    }
}