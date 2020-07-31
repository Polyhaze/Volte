using System;
using System.Text.Json.Serialization;

namespace BrackeysBot
{
    public struct TemporaryInfraction
    {
        [JsonPropertyName("type")]
        public TemporaryInfractionType Type { get; set; }
        [JsonPropertyName("expire")]
        public DateTime Expire { get; set; }

        public static TemporaryInfraction Create(TemporaryInfractionType type, DateTime expire)
            => new TemporaryInfraction(type, expire);

        private TemporaryInfraction(TemporaryInfractionType type, DateTime expire)
        {
            Type = type;
            Expire = expire;
        }
    }
}
