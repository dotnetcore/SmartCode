using SmartCode.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Generator
{
    public interface IDbTypeConverter : IPlugin
    {
        String LanguageType(DbProvider dbProvider, String lang, String dbType);
        String DbType(DbProvider dbProvider, String lang, String languageType);
    }
}
