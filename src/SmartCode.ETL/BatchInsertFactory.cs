using SmartSql.Bulk;
using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql;

namespace SmartCode.ETL
{
    public class BatchInsertFactory
    {
        public static IBulkInsert Create(ISqlMapper sqlMapper, Db.DbProvider dbProvider)
        {
            switch (dbProvider)
            {
                case Db.DbProvider.MySql:
                case Db.DbProvider.MariaDB:
                    {
                        return new SmartSql.Bulk.MySql.BulkInsert(sqlMapper.SessionStore.LocalSession);
                    }
                case Db.DbProvider.PostgreSql:
                    {
                        return new SmartSql.Bulk.PostgreSql.BulkInsert(sqlMapper.SessionStore.LocalSession);
                    }
                case Db.DbProvider.SqlServer:
                    {
                        return new SmartSql.Bulk.SqlServer.BulkInsert(sqlMapper.SessionStore.LocalSession);
                    }
                default:
                    {
                        throw new SmartCodeException($"can not support DbProvider:{dbProvider}!");
                    }
            }
        }
    }
}
