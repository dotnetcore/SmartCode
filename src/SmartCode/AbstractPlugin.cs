using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    [Obsolete]
    public abstract class AbstractPlugin : IPlugin
    {
        public const string NAME_KEY = "Name";
        public const string LOGGER_KEY = "Logger";
        public const string PROJECT_CONFIG_KEY = "Project";
        public const string PLUGIN_MANAGER_KEY = "PluginManager";
        public const string LOGGER_FACTORY_KEY = "LoggerFactory";

        public bool Initialized { get; protected set; } = false;
        public virtual string Name { get; protected set; }
        public ILogger Logger { get; protected set; }
        public ILoggerFactory LoggerFactory { get; protected set; }
        public Project Project { get; protected set; }
        public IPluginManager PluginManager { get; protected set; }
        public virtual void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                
            }
            this.Initialized = true;
        }
    }
}
