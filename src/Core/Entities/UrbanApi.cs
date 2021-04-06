using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    internal class UrbanApiResponse
    {
        [JsonPropertyName("list")]
        // ReSharper disable once CollectionNeverUpdated.Global
        // this property is loaded via System.Text.Json; that's why there's a public setter.
        public List<UrbanEntry> Entries { get; set; }
    }

    public class UrbanEntry
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("written_on")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("definition")]
        public string Definition { get; set; }
        [JsonPropertyName("example")]
        public string Example { get; set; }
        [JsonPropertyName("permalink")]
        public string Permalink { get; set; }
        [JsonPropertyName("current_vote")]
        public string CurrentVote { get; set; }
        [JsonPropertyName("defid")]
        public long DefinitionId { get; set; }
        [JsonPropertyName("thumbs_up")]
        public int Upvotes { get; set; }
        [JsonPropertyName("thumbs_down")]
        public int Downvotes { get; set; }

        public int Score => Upvotes - Downvotes;
    }
}