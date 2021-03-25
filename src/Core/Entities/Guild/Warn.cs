using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public sealed class Warn
    {
        [JsonPropertyName("user")]
        public ulong User { get;  set; }
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        [JsonPropertyName("moderator")]
        public ulong Issuer { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Date { get; set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}