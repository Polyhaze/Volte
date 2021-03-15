using Discord.WebSocket;
using System;

namespace Volte.Core.Entities
{
    public sealed class ShardReadyEventArgs : EventArgs
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