using System.Text.Json.Serialization;

namespace Volte.Core.Models.BotConfig
{
    public class Tokens
    {
        public Tokens()
        {
            DiscordToken = "discord bot token here";
            DblToken = "discord bot list token here";
        }
        
        [JsonPropertyName("discord_token")]
        public string DiscordToken { get; set; }
        [JsonPropertyName("dbl_token")]
        public string DblToken { get; set; }

    }
}