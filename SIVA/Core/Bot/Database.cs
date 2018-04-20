using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SIVA.Core.Bot.Services.Database;
using SIVA.Core.Bot.Services;
using System;
using System.IO;
using System.Linq;

namespace SIVA.Core.Bot
{
    public class Database
    {
        private readonly DbContextOptions<SivaContext> options;
        private readonly DbContextOptions<SivaContext> migrateOptions;

        public Database(IBotCredentials creds)
        {
            var builder = new SqliteConnectionStringBuilder(creds.Db.ConnectionString);
            builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
        }
    }
}