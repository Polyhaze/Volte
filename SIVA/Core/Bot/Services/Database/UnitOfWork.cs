using SIVA.Core.Bot.Services.Database.Repositories;
using SIVA.Core.Bot.Services.Database.Repositories.Impl;
using System;
using System.Threading.Tasks;


namespace SIVA.Core.Bot.Services.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public SivaContext Context { get; }

        private IBotConfigRepository _botConfig;
        public IBotConfigRepository BotConfig => _botConfig ?? (_botConfig = new BotConfigRepository(Context));
        
        private IGuildConfigRepository _guildConfig;
        public IGuildConfigRepository GuildConfig => _guildConfig ?? (_guildConfig = new GuildConfigRepository(Context));
        
        private IDiscordUserRepository _discordUser;
        public IDiscordUserRepository DiscordUsers => _discordUser ?? (_discordUser = new DiscordUserRepository(Context));

        public UnitOfWork(SivaContext ctx) => Context = ctx;

        public int Complete() =>
            Context.SaveChanges();
        
        public Task<int> CompleteAsync() => 
            Context.SaveChangesAsync();
        
        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    Context.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}