using System.Text.Json;
using Gommon;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class Tag
    {
        public string Name { get; set; }
        public string Response { get; set; }
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public long Uses { get; set; }

        public string SanitizeContent() 
            => Response
                .Replace("@everyone", "@\u200Beveryone")
                .Replace("@here", "@\u200Bhere");

        public string FormatContent(VolteContext ctx) 
            => SanitizeContent()
                .ReplaceIgnoreCase("{GuildName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{MemberString}", ctx.Member.AsPrettyString())
                .ReplaceIgnoreCase("{MemberName}", ctx.Member.Username)
                .ReplaceIgnoreCase("{MemberMention}", ctx.Member.Mention)
                .ReplaceIgnoreCase("{MemberDisplayName}", ctx.Member.DisplayName)
                .ReplaceIgnoreCase("{GuildOwnerMention}", ctx.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{MemberTag}", ctx.Member.Discriminator);
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}