using Discord.WebSocket;
using Newtonsoft.Json;

namespace Volte.Core.Models.BotConfig
{
    public sealed class GuildLogging
    {
        internal GuildLogging()
        {
            Enabled = false;
            GuildId = ulong.MinValue;
            ChannelId = ulong.MinValue;
        }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }


        public bool EnsureValidConfiguration(DiscordShardedClient client, out SocketTextChannel channel)
        {
            if (!Enabled)
            {
                channel = null;
                return false;
            }

            if (GuildId is 0 || ChannelId is 0)
            {
                channel = null;
                return false;
            }

            var g = client.GetGuild(GuildId);
            channel = g?.GetTextChannel(ChannelId);
            if (g is null)
            {
                return false;
            }

            if (channel is null)
            {
                return false;
            }

            return true;

        }

    }
}