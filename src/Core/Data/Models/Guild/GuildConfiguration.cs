using Newtonsoft.Json;

namespace Volte.Core.Data.Models.Guild
{
    public sealed class GuildConfiguration
    {
        internal GuildConfiguration()
        {
            Moderation = new ModerationOptions();
            Welcome = new WelcomeOptions();
        }

        [JsonProperty("autorole")]
        public ulong Autorole { get; set; }

        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("welcome_options")]
        public WelcomeOptions Welcome { get; set; }

        [JsonProperty("moderation_options")]
        public ModerationOptions Moderation { get; set; }

        [JsonProperty("delete_message_on_command")]
        public bool DeleteMessageOnCommand { get; set; }
    }
}