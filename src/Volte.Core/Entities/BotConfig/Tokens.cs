using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public class Tokens
    {
        [JsonPropertyName("discord_token")]
        public string Discord { get; set; }
    }
}