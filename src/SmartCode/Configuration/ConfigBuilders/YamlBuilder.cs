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
        private readonly IDeserializer _deserializer;
        public YamlBuilder(string configPath):base(configPath)
        {
            _deserializer = new DeserializerBuilder().Build();
        }

        protected override Project Deserialize(string content)
        {
           return  _deserializer.Deserialize<Project>(content);
        }
    }
}
