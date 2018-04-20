using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using SIVA.Core.Bot.Services.Database.DbTypes;
using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SIVA.Core.Bot.Services.Database
{
    public class SivaContextFactory : IDesignTimeDbContextFactory<SivaContext>
    {
        public SivaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SivaContext>();
            var builder = new SqliteConnectionStringBuilder("Data Source=Resources/SIVA.db");
            builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
            var ctx = new SivaContext(optionsBuilder.Options);
            return ctx;
        }
    }
}