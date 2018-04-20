using Microsoft.EntityFrameworkCore;
using System.Linq;
using SIVA.Core.Bot.Services.Database.DbTypes;

namespace SIVA.Core.Bot.Services.Database
{
    public class SivaContext : DbContext
    {
        public DbSet<GuildConfig> GuildConfigs { get; set; }
        public DbSet<BotConfig> BotConfig { get; set; }
        public DbSet<DiscordUser> DiscordUsers { get; set; }

        public SivaContext(DbContextOptions<SivaContext> options) : base(options)
        {   
        }
    }
}