using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace SmartCode.Configuration.ConfigBuilders
{
    public class YamlBuilder : ConfigBuilder
    {
        private readonly string _configPath;
        private readonly IDeserializer _deserializer;
        public YamlBuilder(string configPath)
        {
            _configPath = configPath;
            _deserializer = new DeserializerBuilder().Build();
        }

        public override Project Build()
        {
            using (StreamReader configStream = new StreamReader(_configPath))
            {
                var yamlConfigStr = configStream.ReadToEnd();
                Project = _deserializer.Deserialize<Project>(yamlConfigStr);
            }
            InitDefault();
            return Project;
        }
    }
}
