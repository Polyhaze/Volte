using System.Collections.Generic;
using Newtonsoft.Json;

namespace Volte.Data.Models.Guild
{
    public sealed class ModerationOptions
    {
        internal ModerationOptions()
            => Blacklist = new List<string>();

        [JsonProperty("mass_ping_checks")]
        public bool MassPingChecks { get; set; }

        [JsonProperty("antilink")]
        public bool Antilink { get; set; }

        [JsonProperty("mod_log_channel")]
        public ulong ModActionLogChannel { get; set; }

        [JsonProperty("mod_role")]
        public ulong ModRole { get; set; }

        [JsonProperty("admin_role")]
        public ulong AdminRole { get; set; }

        [JsonProperty("blacklist")]
        public List<string> Blacklist { get; set; }
    }

    public sealed class WelcomeOptions
    {
        [JsonProperty("welcome_channel")]
        public ulong WelcomeChannel { get; set; }

        [JsonProperty("welcome_message")]
        public string WelcomeMessage { get; set; }

        [JsonProperty("leaving_message")]
        public string LeavingMessage { get; set; }

        [JsonProperty("welcome_color")]
        public uint WelcomeColor { get; set; }
    }
}