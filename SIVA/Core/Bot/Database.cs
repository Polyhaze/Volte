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
            
            var optionsBuilder = new DbContextOptionsBuilder<SivaContext>();
            optionsBuilder.UseSqlite(builder.ToString());
            options = optionsBuilder.Options;
            
            optionsBuilder = new DbContextOptionsBuilder<SivaContext>();
            optionsBuilder.UseSqlite(builder.ToString(), x => x.SuppressForeignKeyEnforcement());
            migrateOptions = optionsBuilder.Options;
        }
        
        public SivaContext GetDbContext()
        {
            var context = new SivaContext(options);
            if (context.Database.GetPendingMigrations().Any())
            {
                var mContext = new SivaContext(migrateOptions);
                mContext.Database.Migrate();
                mContext.SaveChanges();
                mContext.Dispose();
            }
            context.Database.SetCommandTimeout(60);

            //set important sqlite stuffs
            var conn = context.Database.GetDbConnection();
            conn.Open();

            context.Database.ExecuteSqlCommand("PRAGMA journal_mode=WAL");
            using (var com = conn.CreateCommand())
            {
                com.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=OFF";
                com.ExecuteNonQuery();
            }
            return context;
        }

        public IUnitOfWork UnitOfWork =>
            new UnitOfWork(GetDbContext());
    }
}
