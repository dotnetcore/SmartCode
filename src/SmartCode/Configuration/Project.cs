using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace SmartCode.Configuration
{
    public class Project
    {
        public String Module { get; set; }
        public String Author { get; set; }
        public DataSource DataSource { get; set; }
        public String TemplateEngine { get; set; } = "Razor";
        public String Language { get; set; } = "CSharp";
        public Output Output { get; set; }
        public IDictionary<String, object> Paramters { get; set; }
        [YamlMember(Alias = "Build")]
        public IDictionary<string, Build> BuildTasks { get; set; }
        public String OutputPath { get { return Output.Path; } }
    }
}
