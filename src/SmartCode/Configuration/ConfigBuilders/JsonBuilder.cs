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
        private readonly string _configPath;

        public JsonBuilder(string configPath)
        {
            _configPath = configPath;
        }
        public override Project Build()
        {
            using (StreamReader configStream = new StreamReader(_configPath))
            {
                var jsonConfigStr = configStream.ReadToEnd();
                Project = JsonConvert.DeserializeObject<Project>(jsonConfigStr);
            }
            InitDefault();
            return Project;
        }
    }
}
