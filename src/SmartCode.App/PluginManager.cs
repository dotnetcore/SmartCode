using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SmartCode.App
{
    public class PluginManager : IPluginManager
    {
        private readonly SmartCodeOptions _smartCodeOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginManager> _logger;

        public PluginManager(
            SmartCodeOptions smartCodeOptions
            , IServiceProvider serviceProvider
            , ILogger<PluginManager> logger)
        {
            _smartCodeOptions = smartCodeOptions;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public TPlugin Resolve<TPlugin>(string name = "") where TPlugin : IPlugin
        {
            var plugins = ResolvePlugins<TPlugin>();
            IPlugin plugin = default;
            if (plugins.Count() == 0)
            {
                var errMsg = $"Can not find any Plugin:{typeof(TPlugin).Name}";
                _logger.LogError(errMsg);
                throw new SmartCodeException(errMsg);
            }
            if (String.IsNullOrEmpty(name))
            {
                plugin = plugins.FirstOrDefault();
            }
            else
            {
                plugin = plugins.FirstOrDefault(m => String.Equals(m.Name, name, StringComparison.CurrentCultureIgnoreCase));
                if (plugin == null)
                {
                    plugin = plugins.First();
                    var errMsg = $"Can not find Plugin:{typeof(TPlugin).Name},Name:{name},Use Default Plugin:{plugin.GetType().FullName}";
                    _logger.LogWarning(errMsg);
                }
            }
            _logger.LogDebug($"GetPlugin Name:{name},PluginType:{typeof(TPlugin).FullName},ImplType:{plugin.GetType().FullName}!");
            return (TPlugin)plugin;
        }

        private IEnumerable<TPlugin> ResolvePlugins<TPlugin>() where TPlugin : IPlugin
        {
            var plugins = _serviceProvider.GetServices<TPlugin>();
            foreach (var plugin in plugins)
            {
                lock (this)
                {
                    if (!plugin.Initialized)
                    {
                        var pluginType = plugin.GetType();
                        var names = pluginType.AssemblyQualifiedName.Split(',');
                        var typeName = names[0].Trim();
                        var assName = names[1].Trim();
                        var pluginConfig = _smartCodeOptions
                            .Plugins
                            .FirstOrDefault(m => m.ImplAssemblyName == assName && m.ImplTypeName == typeName);
                        plugin.Initialize(pluginConfig.Parameters);
                    }
                }
            }
            return plugins;

        }
    }
}
