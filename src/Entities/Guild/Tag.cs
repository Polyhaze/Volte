using System.Text.Json.Serialization;
using Discord;
using Gommon;
using Volte.Commands;
using Volte.Helpers;

namespace Volte.Entities
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

        public string SanitizedContent => Response
                .Replace("@everyone", $"@{DiscordHelper.Zws}everyone")
                .Replace("@here", $"@{DiscordHelper.Zws}here");

        public string FormatContent(VolteContext ctx)
            => SanitizedContent
                .ReplaceIgnoreCase("{ServerName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", ctx.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", ctx.User.Username)
                .ReplaceIgnoreCase("{UserMention}", ctx.User.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", ctx.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", ctx.User.Discriminator);

        public EmbedBuilder AsEmbed(VolteContext ctx) => ctx.CreateEmbedBuilder(FormatContent(ctx))
            .WithAuthor(author: null)
            .WithFooter($"Requested by {ctx.User}.", ctx.User.GetEffectiveAvatarUrl());


        public override string ToString() => this.AsJson();
    }
}