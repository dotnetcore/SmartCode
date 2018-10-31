using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class PluginConfig
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public String TypeName { get { return Type.Split(',')[0].Trim(); } }
        public String AssemblyName { get { return Type.Split(',')[1].Trim(); } }

        public String ImplType { get; set; }
        public String ImplTypeName { get { return ImplType.Split(',')[0].Trim(); } }
        public String ImplAssemblyName { get { return ImplType.Split(',')[1].Trim(); } }
        public IDictionary<String, object> Paramters { get; set; }
    }
}
