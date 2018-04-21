using System;
using System.Linq;
using SIVA.Core.Bot.Services.Database.DbTypes;
using Microsoft.EntityFrameworkCore;

namespace SIVA.Core.Bot.Services.Database.Repositories.Impl
{
    public class BotConfigRepository : Repository<BotConfig>, IBotConfigRepository
    {
        public BotConfigRepository(DbContext context) : base(context) {}

        public BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null)
        {
            BotConfig config;

            if (includes == null)
            {
                config = _set.Include(bc => bc.Token)
                             .Include(bc => bc.Prefix)
                             .Include(bc => bc.Debug)
                             .Include(bc => bc.BotGameToSet)
                             .Include(bc => bc.TwitchStreamer)
                             .Include(bc => bc.BotOwner)
                             .Include(bc => bc.DefaultEmbedColour)
                             .Include(bc => bc.IsSelfbot)
                             .Include(bc => bc.CurrencySymbol)
                             .Include(bc => bc.FeedbackChannelId)
                             .Include(bc => bc.ErrorEmbedColour)
                             .Include(bc => bc.LogSeverity)
                             .Include(bc => bc.Blacklist)
                             .FirstOrDefault();
            }
            else
            {
                config = includes(_set).FirstOrDefault();
            }

            if (config == null)
            {
                _set.Add(config = new BotConfig());
                _context.SaveChanges();
            }

            return config;
        }
    }
}