using System;
using System.Threading.Tasks;
using SIVA.Core.Bot.Services.Database.Repositories;

namespace SIVA.Core.Bot.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        SivaContext Context { get; }
        
        IBotConfigRepository BotConfig { get; }
        IDiscordUserRepository DiscordUsers { get; }
        IGuildConfigRepository GuildConfig { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}