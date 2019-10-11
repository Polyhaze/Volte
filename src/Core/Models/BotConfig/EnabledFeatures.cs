using System.Text.Json.Serialization;

namespace Volte.Core.Models.BotConfig
{
    /// <summary>
    ///     Model that represents enabled/disabled features as defined in your config.
    /// </summary>
    public sealed class EnabledFeatures
    {
        internal EnabledFeatures() { } //restrict non-Volte assembly instantiation

        [JsonPropertyName("log_to_file")]
        public bool LogToFile { get; } = true;
        [JsonPropertyName("antilink")]
        public bool Antilink { get; } = true;
        [JsonPropertyName("blacklist")]
        public bool Blacklist { get; } = true;
        [JsonPropertyName("mod_log")]
        public bool ModLog { get; } = true;
        [JsonPropertyName("welcome")]
        public bool Welcome { get; } = true;
        [JsonPropertyName("autorole")]
        public bool Autorole { get; } = true;
        [JsonPropertyName("ping_checks")]
        public bool PingChecks { get; } = true;
    }
}