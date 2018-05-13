using System;
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Data.Sqlite;
using Ultz.BeagleFramework.Sql;

namespace SIVA.Core.Bot.Internal
{
    public class Database
    {
        internal SqlEngine sqlEngine;
        internal SqlConnector sqlConnector;
        internal SqlRow sqlRow;
        internal SqlTable sqlTable;

        public DbConnection CreateConn()
        {
            sqlConnector.Init("data/SIVA.db");
            return sqlConnector.CreateConnection();
        }

        public bool DbExists()
        {
            return File.Exists("data/SIVA.db");

        }

        public void InitDb()
        {
            
        }
    }
}