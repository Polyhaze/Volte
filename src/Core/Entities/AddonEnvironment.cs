using System;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Services;

namespace Volte.Core.Entities
{
    public class AddonEnvironment
    {
        public IServiceProvider Services { get; }
        public DiscordShardedClient Client { get; }
        public LoggingService Logger { get; }
        public CommandService Commands { get; }
        public DatabaseService Database { get; }

        public AddonEnvironment(IServiceProvider provider)
        {
            Services = provider;
            Client = provider.Get<DiscordShardedClient>();
            Logger = provider.Get<LoggingService>();
            Commands = provider.Get<CommandService>();
            Database = provider.Get<DatabaseService>();
        }
    }
}