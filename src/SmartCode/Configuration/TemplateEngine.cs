using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Configuration
{
    public class TemplateEngine
    {
        public static TemplateEngine Default = new TemplateEngine
        {
            Name = "Razor"
        };
        public String Name { get; set; }
        public String Root { get; set; }
        public String Path { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public String FullPath => System.IO.Path.Combine(Root, Path);
    }
}
