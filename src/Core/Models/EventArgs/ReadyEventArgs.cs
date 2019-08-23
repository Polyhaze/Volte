using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public sealed class ReadyEventArgs : System.EventArgs
    {
        public DiscordSocketClient Client { get; }
        public DiscordShardedClient ShardedClient { get; }

        public ReadyEventArgs(DiscordSocketClient client, DiscordShardedClient shardedClient)
        {
            Client = client;
            ShardedClient = shardedClient;
        }
    }
}