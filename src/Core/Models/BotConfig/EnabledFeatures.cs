using System.Text.Json.Serialization;

namespace Volte.Core.Models.BotConfig
{
    /// <summary>
    ///     Model that represents enabled/disabled features as defined in your config.
    /// </summary>
    public sealed class EnabledFeatures
    {

        [JsonPropertyName("log_to_file")]
        public bool LogToFile { get; set; } = true;
        [JsonPropertyName("antilink")]
        public bool Antilink { get; set; } = true;
        [JsonPropertyName("blacklist")]
        public bool Blacklist { get; set; } = true;
        [JsonPropertyName("mod_log")]
        public bool ModLog { get; set; } = true;
        [JsonPropertyName("welcome")]
        public bool Welcome { get; set; } = true;
        [JsonPropertyName("autorole")]
        public bool Autorole { get; set; } = true;
        [JsonPropertyName("ping_checks")]
        public bool PingChecks { get; set; } = true;
    }
}