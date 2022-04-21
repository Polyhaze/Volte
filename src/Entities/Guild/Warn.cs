using System;
using System.Text.Json.Serialization;
using Gommon;

namespace Volte.Entities
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

        public override string ToString() => this.AsJson();
    }
}