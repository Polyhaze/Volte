using System;
using System.Text.Json.Serialization;
using LiteDB;
using Volte.Commands;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Volte.Core.Entities
{
    public sealed class Reminder
    {
        public static Reminder FromContext(VolteContext ctx, DateTime end, string reminder) => new Reminder
        {
            TargetTime = end,
            CreationTime = ctx.Now,
            CreatorId = ctx.User.Id,
            GuildId = ctx.Guild.Id,
            ChannelId = ctx.Channel.Id,
            MessageId = ctx.Message.Id,
            Value = reminder
        };
        
        [JsonPropertyName("id"), BsonId]
        public long Id { get; set; }
        [JsonPropertyName("target_timestamp")]
        public DateTime TargetTime { get; set; }
        [JsonPropertyName("creation_timestamp")]
        public DateTime CreationTime { get; set; }
        [JsonPropertyName("creator")]
        public ulong CreatorId { get; set; }
        [JsonPropertyName("guild")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("channel")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("message")]
        public ulong MessageId { get; set; }
        [JsonPropertyName("reminder_for")]
        public string Value { get; set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}