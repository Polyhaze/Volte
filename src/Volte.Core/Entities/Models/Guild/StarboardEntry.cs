using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DSharpPlus.Interactivity.Concurrency;
using LiteDB;

namespace Volte.Core.Entities
{
    public class StarboardEntry
    {
        public StarboardEntry()
        {
            StargazerCollection = new StargazerCollection();
        }

        [JsonPropertyName("starred_users")]
        [BsonRef("stargazers")]
        public StargazerCollection StargazerCollection { get; set; }
        [JsonPropertyName("starred_message_id")]
        public ulong MessageId { get; set; }
        [JsonIgnore]
        public int StarCount => StargazerCollection.Count;
        [JsonPropertyName("starboard_message_id")]
        public ulong StarboardMessageId { get; set; }
    }

    public class StargazerCollection
    {
        public StargazerCollection()
        {
            Stargazers = new Dictionary<ulong, StarTarget>();
        }

        [JsonPropertyName("starred_message_id")]
        public ulong MessageId { get; set; }
        [JsonPropertyName("stargazers")]
        public Dictionary<ulong, StarTarget> Stargazers { get; set; }
        [JsonIgnore]
        public int Count => Stargazers.Count;
    }

    public enum StarTarget : byte
    {
        OriginalMessage, StarboardMessage
    }
}