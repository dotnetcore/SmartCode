using System.Linq;
using System.Collections.Generic;
using SmartCode.Utilities;
using Microsoft.Extensions.Logging;
using SmartCode.Db;

namespace SmartCode.Generator.DbTypeConverter
{
    public class DefaultDbTypeConverter : IDbTypeConverter
    {
        private readonly ILogger _logger;
        private string _xmlPath = AppPath.Relative("DbTypeConverter/DbTypeMap.xml");
        public DbTypeMap DbTypeMap { get; private set; }

        public bool Initialized { get; private set; }

        public string Name => "DbTypeConverter";

        public DefaultDbTypeConverter(ILogger<DefaultDbTypeConverter> logger)
        {
            _logger = logger;
        }

        private void LoadMap()
        {
            _logger.LogDebug($"DbTypeConverter Load DbTypeMap:{_xmlPath} Start!");
            DbTypeMap = XmlConvert.Deserialize<DbTypeMap>(_xmlPath);
            _logger.LogDebug($"DbTypeConverter Load DbTypeMap:{_xmlPath} End!");
        }

        public string LanguageType(DbProvider dbProvider, string lang, string dbType)
        {
            var databaseMap = DbTypeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.LogError($"Can not find DatabaseMap:DbProvider:{dbProvider},Language:{lang}!");
            }

            var dbTypeMap = databaseMap?.DbTypes?.FirstOrDefault(m => m.Name == dbType);

            if (dbTypeMap == null)
            {
                _logger.LogError($"Can not find DatabaseMap:DbProvider:{dbProvider},Language:{lang},DbType:{dbType}!");
            }

            return dbTypeMap?.To;
        }

        public string DbType(DbProvider dbProvider, string lang, string languageType)
        {
            var databaseMap = DbTypeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.LogError($"Can not find DatabaseMap:DbProvider:{dbProvider},Language:{lang}!");
            }

            var dbTypeMap = databaseMap?.DbTypes?.FirstOrDefault(m => m.To == languageType);

            if (dbTypeMap == null)
            {
                _logger.LogError(
                    $"Can not find DatabaseMap:DbProvider:{dbProvider},Language:{lang},LanguageType:{languageType}!");
            }

            return dbTypeMap?.To;
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                if (parameters.Value("XmlPath", out string xmlPath))
                {
                    _xmlPath = xmlPath;
                }
            }

            LoadMap();
            Initialized = true;
        }
    }
}