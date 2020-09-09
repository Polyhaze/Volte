using System.Text.Json;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public sealed class GuildConfiguration
    {
        internal GuildConfiguration()
        {
            Moderation = new ModerationOptions();
            Welcome = new WelcomeOptions();
            Starboard = new StarboardOptions();
        }

        [JsonPropertyName("autorole")]
        public ulong Autorole { get; set; }

        [JsonPropertyName("command_prefix")]
        public string CommandPrefix { get; set; }

        [JsonPropertyName("welcome_options")]
        public WelcomeOptions Welcome { get; set; }

        [JsonPropertyName("moderation_options")]
        public ModerationOptions Moderation { get; set; }
        
        [JsonPropertyName("starboard_options")]
        public StarboardOptions Starboard { get; set; }

        [JsonPropertyName("delete_message_on_command")]
        public bool DeleteMessageOnCommand { get; set; }

        [JsonPropertyName("delete_message_on_tag_command_invocation")]
        public bool DeleteMessageOnTagCommandInvocation { get; set; }

        [JsonPropertyName("embed_tags_and_show_its_author")]
        public bool EmbedTagsAndShowAuthor { get; set; }
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);

    }
}