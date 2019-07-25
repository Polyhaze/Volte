using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Volte.Core.Data.Models.BotConfig
{
    /// <summary>
    ///     Model that represents enabled/disabled features as defined in your config.
    /// </summary>
    public sealed class EnabledFeatures
    {
        internal EnabledFeatures() { } //restrict non-Volte assembly instantiation

        [JsonProperty("antilink")]
        public bool Antilink { get; } = true;
        [JsonProperty("blacklist")]
        public bool Blacklist { get; } = true;
        [JsonProperty("mod_log")]
        public bool ModLog { get; } = true;
        [JsonProperty("welcome")]
        public bool Welcome { get; } = true;
        [JsonProperty("autorole")]
        public bool Autorole { get; } = true;
        [JsonProperty("ping_checks")]
        public bool PingChecks { get; } = true;
    }
}