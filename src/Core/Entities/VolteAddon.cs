using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public class VolteAddonInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}