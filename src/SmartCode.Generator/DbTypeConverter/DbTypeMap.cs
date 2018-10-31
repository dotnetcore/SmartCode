using SmartCode.Db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmartCode.Generator.DbTypeConverter
{
    public class DbTypeMap
    {
        [XmlElement("Database")]
        public List<DatabaseMap> Databases { get; set; }
    }

    public class DatabaseMap
    {
        [XmlAttribute]
        public DbProvider DbProvider { get; set; }
        [XmlAttribute]
        public String Language { get; set; }
        [XmlElement("DbType")]
        public List<DbTypeMapLange> DbTypes { get; set; }
    }
    public class DbTypeMapLange
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String To { get; set; }
    }
}
