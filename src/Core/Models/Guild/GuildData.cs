using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.WebSocket;

namespace Volte.Core.Models.Guild
{
    public sealed class GuildData
    {
        public GuildData()
        {
            Configuration = new GuildConfiguration();
            Extras = new GuildExtras();
        }

        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("owner")]
        public ulong OwnerId { get; set; }

        [JsonPropertyName("configuration")]
        public GuildConfiguration Configuration { get; set; }

        [JsonPropertyName("extras")]
        public GuildExtras Extras { get; set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
    }
}