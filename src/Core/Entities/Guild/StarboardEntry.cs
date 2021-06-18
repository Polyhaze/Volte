using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public class StarboardEntryBase
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("key")]
        public ulong Key { get; set; }
        
        [JsonPropertyName("value")]
        public StarboardEntry2 Value { get; set; }
    }
    
    public class StarboardEntry2
    {
        public StarboardEntry2()
        {
            Stargazers = new Dictionary<ulong, StarTarget>();
        }

        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        
        [JsonPropertyName("starred_message_id")]
        public ulong StarredMessageId { get; set; }
        
        [JsonPropertyName("starboard_message_id")]
        public ulong StarboardMessageId { get; set; }
        
        [JsonPropertyName("stargazers")]
        public Dictionary<ulong, StarTarget> Stargazers { get; set; }
        
        [JsonIgnore]
        public int StarCount => Stargazers.Count;
    }

    public enum StarTarget : byte
    {
        OriginalMessage, StarboardMessage
    }
}