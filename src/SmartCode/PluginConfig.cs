using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class PluginConfig
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public String TypeName => Type.Split(',')[0].Trim();
        public String AssemblyName => Type.Split(',')[1].Trim();

        public String ImplType { get; set; }
        public String ImplTypeName => ImplType.Split(',')[0].Trim();
        public String ImplAssemblyName => ImplType.Split(',')[1].Trim();
        public IDictionary<String, object> Parameters { get; set; }
    }
}
