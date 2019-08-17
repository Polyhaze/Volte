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
    }
}