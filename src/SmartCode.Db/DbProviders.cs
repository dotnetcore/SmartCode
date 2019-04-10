using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCode.Db
{
    public class DbProviders
    {
        private static readonly IDictionary<string, SmartSql.Configuration.DbProvider> _dbProviders;

        static DbProviders()
        {
            _dbProviders = new Dictionary<string, SmartSql.Configuration.DbProvider>(StringComparer.CurrentCultureIgnoreCase);
            InitDbProviders();
        }
        private static void InitDbProviders()
        {
            _dbProviders.Add("MySql", new SmartSql.Configuration.DbProvider
            {
                Name = "MySql",
                ParameterPrefix = "?",
                Type = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data"
            });
            _dbProviders.Add("MariaDB", new SmartSql.Configuration.DbProvider
            {
                Name = "MariaDB",
                ParameterPrefix = "?",
                Type = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data"
            });
            _dbProviders.Add("PostgreSql", new SmartSql.Configuration.DbProvider
            {
                Name = "PostgreSql",
                ParameterPrefix = "@",
                Type = "Npgsql.NpgsqlFactory,Npgsql"
            });
            _dbProviders.Add("SqlServer", new SmartSql.Configuration.DbProvider
            {
                Name = "SqlServer",
                ParameterPrefix = "@",
                Type = "System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient"
            });
            _dbProviders.Add("Oracle", new SmartSql.Configuration.DbProvider
            {
                Name = "Oracle",
                ParameterPrefix = ":",
                Type = "Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess"
            });
            _dbProviders.Add("SQLite", new SmartSql.Configuration.DbProvider
            {
                Name = "SQLite",
                ParameterPrefix = "$",
                Type = "System.Data.SQLite.SQLiteFactory,System.Data.SQLite"
            });
        }
        public static SmartSql.Configuration.DbProvider GetDbProvider(string providerName)
        {
            if (!_dbProviders.TryGetValue(providerName, out SmartSql.Configuration.DbProvider dbProvider))
            {
                var supportDbProviders = String.Join(",", _dbProviders.Select(m => m.Key));
                throw new SmartCodeException($"Can not find DbProvider:{providerName},SmartCode support DbProviders:{supportDbProviders}!");
            }
            return dbProvider;
        }
    }
}
