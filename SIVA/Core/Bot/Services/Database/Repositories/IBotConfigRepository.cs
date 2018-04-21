using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using SIVA.Core.Bot.Services.Database.DbTypes;

namespace SIVA.Core.Bot.Services.Database.Repositories
{
    public interface IBotConfigRepository : IRepository<BotConfig>
    {
        BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null);
    }
}