using System.Collections.Generic;
using Newtonsoft.Json;

namespace Volte.Core.Models
{
    public sealed class BinData
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("expiration")]
        public int Expiration { get; set; }

        [JsonProperty("files")]
        public List<BinFile> Files { get; set; }
    }

    public sealed class BinFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}