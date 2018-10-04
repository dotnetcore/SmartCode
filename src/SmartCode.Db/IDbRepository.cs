using SmartCode.Configuration;
using SmartCode.Db.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Db
{
    public interface IDbRepository
    {
        IEnumerable<Table> QueryTable();
    }
}
