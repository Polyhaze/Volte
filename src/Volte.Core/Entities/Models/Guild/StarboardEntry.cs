using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DSharpPlus.Interactivity.Concurrency;

namespace Volte.Core.Entities
{
    public class StarboardEntry
    {
        public StarboardEntry()
        {
            StarredUsers = new Dictionary<ulong, StarTarget>();
        }

        [JsonPropertyName("starred_users")]
        public Dictionary<ulong, StarTarget> StarredUsers { get; set; }
        [JsonPropertyName("starred_message_id")]
        public ulong MessageId { get; set; }
        [JsonIgnore]
        public int StarCount => StarredUsers.Count;
        [JsonPropertyName("starboard_message_id")]
        public ulong StarboardMessageId { get; set; }
    }

    public enum StarTarget : byte
    {
        OriginalMessage, StarboardMessage
    }
}