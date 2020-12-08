using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SmartCode.Configuration;
using System.IO;
using SmartCode.Configuration.ConfigBuilders;
using System.Reflection;
using System.Text;
using HandlebarsDotNet;
using SmartCode.ETL;
using SmartCode.Generator;

namespace SmartCode.App
{
    public class SmartCodeApp
    {
        public SmartCodeOptions SmartCodeOptions { get; }
        public String AppDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public IConfigBuilder ConfigBuilder { get; private set; }
        public IServiceCollection Services => SmartCodeOptions.Services;
        public IServiceProvider ServiceProvider { get; private set; }
        public Project Project { get; private set; }
        public string ConfigPath => SmartCodeOptions.ConfigPath;
        public ILogger<SmartCodeApp> Logger { get; private set; }

        public SmartCodeApp(SmartCodeOptions smartCodeOptions)
        {
            SmartCodeOptions = smartCodeOptions;
            BuildProject();
            RegisterServices();
        }

        private void BuildProject()
        {
            var pathExtension = Path.GetExtension(ConfigPath).ToUpper();
            switch (pathExtension)
            {
                case ".JSON":
                {
                    ConfigBuilder = new JsonBuilder(ConfigPath);
                    break;
                }

                case ".YML":
                {
                    ConfigBuilder = new YamlBuilder(ConfigPath);
                    break;
                }

                default:
                {
                    throw new SmartCodeException($"ConfigPath:{ConfigPath},未知扩展名：{pathExtension}");
                }
            }

            Project = ConfigBuilder.Build();
            Project.ConfigPath = ConfigPath;
        }

        private void RegisterServices()
        {
            Services.AddSingleton<SmartCodeOptions>(SmartCodeOptions);
            Services.AddSingleton<Project>(Project);
            RegisterPlugins();
            Services.AddSingleton<IPluginManager, PluginManager>();
            if (Project.Mode == Project.ProjectMode.ETL)
            {
                Services.AddSingleton<IProjectBuilder, ETLProjectBuilder>();
            }
            else
            {
                if (Project.DataSource.Parameters.ContainsKey("Query"))
                {
                    Services.AddSingleton<IProjectBuilder, ETLProjectBuilder>();
                }
                else
                {
                    Services.AddSingleton<IProjectBuilder, GeneratorProjectBuilder>();
                }
            }
            ServiceProvider = Services.BuildServiceProvider();
            Logger = ServiceProvider.GetRequiredService<ILogger<SmartCodeApp>>();
        }

        private void RegisterPlugins()
        {
            foreach (var plugin in SmartCodeOptions.Plugins)
            {
                var pluginType = Assembly.Load(plugin.AssemblyName).GetType(plugin.TypeName);
                if (pluginType == null)
                {
                    throw new SmartCodeException($"Plugin.Type:{plugin.TypeName} can not find!");
                }

                var implType = Assembly.Load(plugin.ImplAssemblyName).GetType(plugin.ImplTypeName);
                if (implType == null)
                {
                    throw new SmartCodeException($"Plugin.ImplType:{plugin.ImplTypeName} can not find!");
                }

                if (!pluginType.IsAssignableFrom(implType))
                {
                    throw new SmartCodeException(
                        $"Plugin.ImplType:{implType.FullName} can not Impl Plugin.Type：{pluginType.FullName}!");
                }

                Services.AddSingleton(pluginType, implType);
            }
        }

        public async Task Run()
        {
            try
            {
                Handlebars.Configuration.TextEncoder = NullTextEncoder.Instance;
                var projectBuilder = ServiceProvider.GetRequiredService<IProjectBuilder>();
                Stopwatch stopwatch = Stopwatch.StartNew();
                Logger.LogInformation($"------- Build ConfigPath:{ConfigPath} Start! --------");
                await projectBuilder.Build();
                Logger.LogInformation(
                    $"-------- Build ConfigPath:[{ConfigPath}],Output:[{Project.Output?.Path}],Taken:[{stopwatch.ElapsedMilliseconds}ms] End! --------");
            }
            catch (SmartCodeException scEx)
            {
                Logger.LogError(new EventId(scEx.HResult), scEx, scEx.Message);
                throw;
            }
        }

        public class NullTextEncoder : ITextEncoder
        {
            public static NullTextEncoder Instance = new NullTextEncoder();

            public string Encode(string value)
            {
                return value;
            }

            public void Encode(StringBuilder text, TextWriter target)
            {
                target.Write(text.ToString());
            }

            public void Encode(string text, TextWriter target)
            {
                target.Write(text);
            }

            public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
            {
                while (text.MoveNext())
                {
                    target.Write(text.Current);
                }
            }

        }
    }
}
