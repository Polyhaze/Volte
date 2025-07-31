using System.Text.Json.Serialization;

namespace Volte.Systems.Database.Entities;

public sealed class Tag
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("aliases")] 
    public List<string> Aliases { get; set; } = [];

    [JsonPropertyName("content")]
    public string Response { get; set; }

    [JsonPropertyName("creator")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("guild")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("uses")]
    public long Uses { get; set; }


    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}