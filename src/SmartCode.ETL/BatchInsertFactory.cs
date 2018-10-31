using SmartSql.Batch;
using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;

namespace SmartCode.ETL
{
    public class BatchInsertFactory
    {
        public static IBatchInsert Create(ISmartSqlMapper smartSqlMapper, Db.DbProvider dbProvider)
        {
            switch (dbProvider)
            {
                case Db.DbProvider.MySql:
                case Db.DbProvider.MariaDB:
                    {
                        return new SmartSql.Batch.MySql.BatchInsert(smartSqlMapper);
                    }
                case Db.DbProvider.PostgreSql:
                    {
                        return new SmartSql.Batch.PostgreSql.BatchInsert(smartSqlMapper);
                    }
                case Db.DbProvider.SqlServer:
                    {
                        return new SmartSql.Batch.SqlServer.BatchInsert(smartSqlMapper);
                    }
                default:
                    {
                        throw new SmartCodeException($"can not support DbProvider:{dbProvider}!");
                    }
            }
        }
    }
}
