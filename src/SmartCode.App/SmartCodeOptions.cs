using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.App
{
    public class SmartCodeOptions
    {
        public string Name { get; set; } = "SmartCode";
        public string Author { get; set; } = "Ahoo Wang";
        public string Version { get; set; } = "1.0.0";
        public string Github { get; set; } = "https://github.com/Ahoo-Wang/SmartCode";
        public IEnumerable<PluginConfig> Plugins { get; set; }
    }
}
