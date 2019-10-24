using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public sealed class ShardReadyEventArgs : System.EventArgs
    {
        public DiscordSocketClient Shard { get; }
        public DiscordShardedClient Client { get; }

        public ShardReadyEventArgs(DiscordSocketClient shard, DiscordShardedClient client)
        {
            Shard = shard;
            Client = client;
        }
    }
}