using System.Text.Json;
using Gommon;
using Volte.Commands;

namespace Volte.Core.Models.Guild
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
                .ReplaceIgnoreCase("{ServerName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", ctx.Member.Username)
                .ReplaceIgnoreCase("{UserMention}", ctx.Member.Mention)
                .ReplaceIgnoreCase("{UserEffectiveName}", ctx.Member.GetEffectiveUsername())
                .ReplaceIgnoreCase("{OwnerMention}", ctx.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", ctx.Member.Discriminator);
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}