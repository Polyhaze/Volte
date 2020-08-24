using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Volte.Core.Models.Guild
{
    public class StarboardEntry
    {
        public StarboardEntry()
        {
            StarredUserIds = new List<ulong>();
        }
        
        [JsonPropertyName("starred_users")]
        public List<ulong> StarredUserIds { get; set; }
        [JsonPropertyName("starred_message_id")]
        public ulong MessageId { get; set; }
        
        [JsonPropertyName("star_count")]
        public int StarCount { get; set; }
        [JsonPropertyName("starboard_message_id")]
        public ulong StarboardMessageId { get; set; }
    }
}