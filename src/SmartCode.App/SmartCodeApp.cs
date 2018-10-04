using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SmartCode.Configuration;
using System.IO;
using SmartCode.Configuration.ConfigBuilders;
using System.Reflection;
using System.Linq;

namespace SmartCode.App
{
    public class SmartCodeApp
    {
        const string APP_SETTINGS_PATH = "appsettings.json";
        const string SMARTCODE_KEY = "SmartCode";
        public IConfigurationRoot Configuration { get; private set; }
        public SmartCodeOptions SmartCodeOptions { get; private set; }
        public String AppDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public IConfigBuilder ConfigBuilder { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public Project Project { get; private set; }
        public string ConfigPath { get; }
        public ILogger<SmartCodeApp> Logger { get; private set; }
        public SmartCodeApp(String configPath)
        {
            ConfigPath = configPath;
            BuildProject();
            InitConfig();
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
                        throw new SmartCodeException($"未知扩展名：{pathExtension}");
                    }
            }
            Project = ConfigBuilder.Build();
        }
        private void InitConfig()
        {
            var appSettingsbuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDirectory)
                    .AddJsonFile(APP_SETTINGS_PATH, false, true);
            Configuration = appSettingsbuilder.Build();
            SmartCodeOptions = Configuration.GetSection(SMARTCODE_KEY).Get<SmartCodeOptions>();
        }

        private void RegisterServices()
        {
            Services = new ServiceCollection();
            Services.AddSingleton<SmartCodeOptions>(SmartCodeOptions);
            Services.AddSingleton<Project>(Project);
            Services.AddLogging((loggerBuilder) =>
            {
                var loggingConfig = Configuration.GetSection("Logging");
                loggerBuilder.AddConfiguration(loggingConfig).AddConsole();
            });
            RegisterPlugins();
            Services.AddSingleton<IPluginManager, PluginManager>();
            Services.AddSingleton<IProjectBuilder, ProjectBuilder>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        private void RegisterPlugins()
        {
            foreach (var plugin in SmartCodeOptions.Plugins)
            {
                var pluginType = Assembly.Load(plugin.AssemblyName).GetType(plugin.TypeName);
                var implType = Assembly.Load(plugin.ImplAssemblyName).GetType(plugin.ImplTypeName);
                if (!pluginType.IsAssignableFrom(implType))
                {
                    throw new SmartCodeException($"Plugin.ImplType:{implType.FullName} can not Impl Plugin.Type：{pluginType.FullName}!");
                }
                Services.AddSingleton(pluginType, implType);
            }
        }

        public async Task Run()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<SmartCodeApp>>();
            var projectBuilder = ServiceProvider.GetRequiredService<IProjectBuilder>();
            logger.LogInformation($"------- Build ConfigPath:{ConfigPath} Start! --------");
            await projectBuilder.Build();
            logger.LogInformation($"-------- Build ConfigPath:{ConfigPath},Output:{Project.OutputPath} End! --------");
        }
    }
}
