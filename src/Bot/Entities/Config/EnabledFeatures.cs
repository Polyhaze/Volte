using System.Text.Json.Serialization;

namespace Volte.Entities;

/// <summary>
///     Model that represents enabled/disabled features as defined in your config.
/// </summary>
public sealed class EnabledFeatures
{
    [JsonPropertyName("log_to_file")]
    public bool LogToFile { get; set; } = true;
    [JsonPropertyName("mod_log")]
    public bool ModLog { get; set; } = true;
    [JsonPropertyName("welcome")]
    public bool Welcome { get; set; } = true;
    [JsonPropertyName("autorole")]
    public bool Autorole { get; set; } = true;
}