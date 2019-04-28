using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartCode.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.CLI
{
    public class DefaultSmartCodeAppBuilder
    {
        const string APP_SETTINGS_PATH = "appsettings.json";
        const string SMARTCODE_KEY = "SmartCode";
        public String AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public SmartCodeApp Build(string configPath)
        {
            var appSettingsbuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDirectory)
                    .AddJsonFile(APP_SETTINGS_PATH, false, true);
            var configuration = appSettingsbuilder.Build();
            var smartCodeOptions = configuration.GetSection(SMARTCODE_KEY).Get<SmartCodeOptions>();
            smartCodeOptions.ConfigPath = configPath;
            smartCodeOptions.Services.AddLogging((loggerBuilder) =>
            {
                var loggingConfig = configuration.GetSection("Logging");
                loggerBuilder.AddConfiguration(loggingConfig).AddConsole();
            });
            return new SmartCodeApp(smartCodeOptions);
        }
    }
}
