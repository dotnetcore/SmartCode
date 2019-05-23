using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.App
{
    public class SmartCodeOptions
    {
        public string Name { get; } = "SmartCode";
        public string Author { get; } = "Ahoo Wang";
        public string Github { get; } = "https://github.com/Ahoo-Wang/SmartCode";
        public String ConfigPath { get; set; }
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        public IEnumerable<PluginConfig> Plugins { get; set; }
    }
}
