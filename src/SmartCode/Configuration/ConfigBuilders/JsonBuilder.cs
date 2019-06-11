using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartCode.Configuration.ConfigBuilders
{
    public class JsonBuilder : ConfigBuilder
    {
        public JsonBuilder(string configPath) : base(configPath)
        {
        }

        protected override Project Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<Project>(content);
        }
    }
}