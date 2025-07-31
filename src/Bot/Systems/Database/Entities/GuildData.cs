using System.Text.Json.Serialization;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.UserFilter;

namespace Volte.Systems.Database.Entities;

public sealed class GuildData
{
    public GuildData()
    {
        Configuration = new();
        Extras = new();
    }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("owner")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("configuration")]
    public GuildConfiguration Configuration { get; set; }

    [JsonPropertyName("extras")]
    public GuildExtras Extras { get; set; }

    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}