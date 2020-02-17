using SmartSql.Bulk;
using System.Collections.Generic;
using SmartSql;

namespace SmartCode.ETL
{
    public class BatchInsertFactory
    {
        public static IBulkInsert Create(ISqlMapper sqlMapper, Db.DbProvider dbProvider, BuildContext buildContext)
        {
            switch (dbProvider)
            {
                case Db.DbProvider.MySql:
                case Db.DbProvider.MariaDB:
                {
                    var bulkInset = new SmartSql.Bulk.MySql.BulkInsert(sqlMapper.SessionStore.LocalSession);
                    if (buildContext.Build.Parameters.Value("SecureFilePriv",
                        out string secureFilePriv))
                    {
                        bulkInset.SecureFilePriv = secureFilePriv;
                    }

                    return bulkInset;
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