using System;
using JsonParser = System.Text.Json.JsonSerializer; // same class in namespace LiteDB
using System.Text.Json.Serialization;
using Volte.Commands;
using LiteDB;
using Volte;
using Volte.Interactions;

namespace Volte.Entities
{
    public sealed class Reminder
    {
        public static Reminder CreateFrom(SlashCommandContext ctx, DateTime end, string reminder) => new Reminder
        {
            TargetTime = end,
            CreationTime = DateTime.Now,
            CreatorId = ctx.User.Id,
            ReminderText = reminder
        };
        
        public static Reminder CreateFrom(VolteContext ctx, DateTime end, string reminder) => new Reminder
        {
            TargetTime = end,
            CreationTime = ctx.Now,
            CreatorId = ctx.User.Id,
            ReminderText = reminder
        };
        
        [BsonId, JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("target_timestamp")]
        public DateTime TargetTime { get; set; }
        [JsonPropertyName("creation_timestamp")]
        public DateTime CreationTime { get; set; }
        [JsonPropertyName("creator")]
        public ulong CreatorId { get; set; }
        [JsonPropertyName("reminder_for")]
        public string ReminderText { get; set; }

        public override string ToString()
            => JsonParser.Serialize(this, Config.JsonOptions);
    }
}