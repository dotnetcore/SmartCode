using SmartCode.Db;
using SmartCode.Generator;
using SmartCode.Generator.DbTypeConverter;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.Logging;
namespace SmartCode.Tests
{
    public class DbTypeConverter_Test
    {
        [Fact]
        public void Convert()
        {
            var logger = SmartSql.Logging.NoneLoggerFactory.Instance.CreateLogger<DefaultDbTypeConverter>();
            var xmlPath = @"E:\Ahoo\SmartCode\src\SmartCode\DbTypeConverter\DbTypeMap.xml";
            var patamters = new Dictionary<string, object> {
                { "XmlPath",xmlPath}
            };
            IDbTypeConverter convert = new DefaultDbTypeConverter(logger);
            convert.Initialize(patamters);
            var langType = convert.LanguageType(DbProvider.SqlServer, "CSharp", "int");
            Assert.Equal("Int16", langType);
        }
    }
}
