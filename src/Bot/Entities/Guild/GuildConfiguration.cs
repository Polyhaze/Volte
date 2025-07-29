using System.Text.Json.Serialization;

namespace Volte.Entities;

public sealed class GuildConfiguration
{
    [JsonPropertyName("autorole")]
    public ulong Autorole { get; set; }

    [JsonPropertyName("command_prefix")]
    public string CommandPrefix { get; set; }

    [JsonPropertyName("welcome_options")]
    public WelcomeOptions Welcome { get; set; } = new();

    [JsonPropertyName("moderation_options")]
    public ModerationOptions Moderation { get; set; } = new();

    [JsonPropertyName("starboard_options")]
    public StarboardOptions Starboard { get; set; } = new();

    [JsonPropertyName("reply_inline")]
    public bool ReplyInline { get; set; }

    [JsonPropertyName("embed_tags_and_show_its_author")]
    public bool EmbedTagsAndShowAuthor { get; set; }
        
    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);

}