using System.Text.Json.Serialization;

namespace Volte.Systems.Addons;

public class VolteAddonMeta
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }
}

public class VolteAddon
{
    public VolteAddonMeta Meta { get; set; }
    public string Script { get; set; }
}