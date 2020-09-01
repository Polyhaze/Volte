using System.Text.Json.Serialization;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using JetBrains.Annotations;

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


        public bool EnsureValidConfiguration([NotNull] DiscordShardedClient client, [CanBeNull] out DiscordChannel channel)
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
            channel = g?.GetChannel(ChannelId);
            if (g is null)
            {
                return false;
            }

            return channel is not null;
        }

    }
}