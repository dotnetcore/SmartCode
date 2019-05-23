using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace SmartCode.Configuration
{
    public class Project
    {
        public String ConfigPath { get; set; }
        public String Module { get; set; }
        public String Author { get; set; }
        public DataSource DataSource { get; set; }
        public TemplateEngine TemplateEngine { get; set; } = TemplateEngine.Default;
        public String Language { get; set; } = "CSharp";
        public Output Output { get; set; }
        public IDictionary<String, object> Parameters { get; set; } = new Dictionary<String, object>();
        [YamlMember(Alias = "Build")]
        public IDictionary<string, Build> BuildTasks { get; set; }
        public String OutputPath => Output.Path;
        public NamingConverter NamingConverter { get; set; }
    }
}
