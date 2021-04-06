using Discord.WebSocket;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public sealed class GuildLogging
    {
        public GuildLogging()
        {
            Enabled = false;
            GuildId = ulong.MinValue;
            ChannelId = ulong.MinValue;
        }
        
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }


        public bool EnsureValidConfiguration(DiscordShardedClient client, out SocketTextChannel channel)
        {
            channel = null;
            if (!Enabled)
            {
                return false;
            }

            if (GuildId is 0 || ChannelId is 0)
            {
                return false;
            }

            var g = client.GetGuild(GuildId);
            channel = g?.GetTextChannel(ChannelId);

            return channel != null;
        }
    }
}