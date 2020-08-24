using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DSharpPlus.Interactivity.Concurrency;

namespace Volte.Core.Models.Guild
{
    public class StarboardEntry
    {
        public StarboardEntry()
        {
            StarredUserIds = new HashSet<ulong>();
        }

        [JsonPropertyName("starred_users")]
        public HashSet<ulong> StarredUserIds { get; set; }
        [JsonPropertyName("starred_message_id")]
        public ulong MessageId { get; set; }
        [JsonIgnore]
        public int StarCount => StarredUserIds.Count;
        [JsonPropertyName("starboard_message_id")]
        public ulong StarboardMessageId { get; set; }
    }
}