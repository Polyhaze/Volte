using Newtonsoft.Json;

namespace Volte.Data.Models.Guild
{
    public sealed class GuildConfiguration
    {
        internal GuildConfiguration()
        {
            ModerationOptions = new ModerationOptions();
            WelcomeOptions = new WelcomeOptions();
        }

        [JsonProperty("autorole")]
        public ulong Autorole { get; set; }

        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("welcome_options")]
        public WelcomeOptions WelcomeOptions { get; set; }

        [JsonProperty("moderation_options")]
        public ModerationOptions ModerationOptions { get; set; }

        [JsonProperty("delete_message_on_command")]
        public bool DeleteMessageOnCommand { get; set; }
    }
}