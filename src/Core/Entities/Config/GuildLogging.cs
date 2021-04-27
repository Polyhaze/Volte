using System.Text.Json.Serialization;
using Discord.WebSocket;

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


        public bool TryValidate(DiscordShardedClient client, out SocketTextChannel channel)
        {
            channel = null;
            if (!Enabled)
                return false;

            if (GuildId is 0 || ChannelId is 0)
                return false;

            channel = client.GetGuild(GuildId)?.GetTextChannel(ChannelId);

            return channel != null;
        }
    }
}