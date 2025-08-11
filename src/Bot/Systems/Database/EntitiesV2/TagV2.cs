namespace Volte.Systems.Database.EntitiesV2;

public class TagV2
{
    public string Name { get; set; }
    
    public List<string> Aliases { get; set; } = [];
    
    public string Response { get; set; }
    
    public ulong CreatorId { get; set; }
    
    public ulong GuildId { get; set; }
    
    public ulong Uses { get; set; }

    public string SanitizeContent()
        => Response
            .Replace("@everyone", $"@{DiscordHelper.Zws}everyone")
            .Replace("@here", $"@{DiscordHelper.Zws}here");

    public string FormatContent(VolteContext ctx)
        => SanitizeContent()
            .Replace("{ServerName}", ctx.Guild.Name)
            .Replace("{GuildName}", ctx.Guild.Name)
            .Replace("{UserName}", ctx.User.Username)
            .Replace("{UserMention}", ctx.User.Mention)
            .Replace("{OwnerMention}", ctx.Guild.Owner.Mention)
            .Replace("{UserTag}", ctx.User.Discriminator);

    public EmbedBuilder AsEmbed(VolteContext ctx) => AsContentEmbed(ctx)
        .WithAuthor(author: null)
        .WithFooter($"Requested by {ctx.User}.", ctx.User.GetEffectiveAvatarUrl());

    public EmbedBuilder AsContentEmbed(VolteContext ctx) => ctx.CreateEmbedBuilder(FormatContent(ctx));


    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}