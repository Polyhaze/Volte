using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BrackeysBot
{
    public class UserData
    {
        [JsonPropertyName("id")]
        public ulong ID { get; set; }

        [JsonPropertyName("temporaryInfractions")]
        public List<TemporaryInfraction> TemporaryInfractions { get; set; } = new List<TemporaryInfraction>();
        [JsonPropertyName("infractions")]
        public List<Infraction> Infractions { get; set; } = new List<Infraction>();

        [JsonPropertyName("eventPoints")]
        public int Points { get; set; }
        
        [JsonPropertyName("stars")]
        public int Stars { get; set; }

        [JsonPropertyName("muted")]
        public bool Muted { get; set; }
        
        public UserData (ulong id)
        {
            ID = id;
        }
        private UserData()
        {
        }

        public bool HasTemporaryInfraction(TemporaryInfractionType type)
            => TemporaryInfractions.Any(i => i.Type == type);
    }
}
