using System;
using System.Text.Json.Serialization;

using Discord;

using Humanizer;

namespace BrackeysBot
{
    public struct Infraction
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("moderator")]
        public ulong Moderator { get; set; }
        [JsonPropertyName("type")]
        public InfractionType Type { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("additionalInfo")]
        public string AdditionalInfo { get; set; }
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        public static Infraction Create(int id)
            => new Infraction
            { 
                ID = id,
                Time = DateTime.UtcNow 
            };

        public Infraction WithModerator(IUser moderator)
        {
            Moderator = moderator.Id;
            return this;
        }
        public Infraction WithType(InfractionType type)
        {
            Type = type;
            return this;
        }
        public Infraction WithDescription(string description)
        {
            Description = description;
            return this;
        }
        public Infraction WithAdditionalInfo(string additionalInfo)
        {
            AdditionalInfo = additionalInfo;
            return this;
        }

        public override string ToString()
            => $"[{ID}] {Description} • {Type.Humanize()} • {Time.Humanize()}";
    }
}
