using System.Text.Json;
using System.Text.Json.Serialization;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("content")]
        public string Response { get; set; }
        [JsonPropertyName("creator")]
        public ulong CreatorId { get; set; }
        [JsonPropertyName("guild")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("uses")]
        public long Uses { get; set; }

        public string SanitizeContent() 
            => Response
                .Replace("@everyone", "@\u200Beveryone")
                .Replace("@here", "@\u200Bhere");

        public string FormatContent(VolteContext ctx) 
            => SanitizeContent()
                .Replace("{ServerName}", ctx.Guild.Name)
                .Replace("{GuildName}", ctx.Guild.Name)
                .Replace("{UserName}", ctx.User.Username)
                .Replace("{UserMention}", ctx.User.Mention)
                .Replace("{OwnerMention}", ctx.Guild.Owner.Mention)
                .Replace("{UserTag}", ctx.User.Discriminator);
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}