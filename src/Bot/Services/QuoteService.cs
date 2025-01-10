using System.Text.RegularExpressions;

namespace Volte.Services;

//thanks discord-csharp/MODiX for the idea and some of the code (definitely the regex lol)
public sealed partial class QuoteService : VolteService
{
    private readonly DiscordSocketClient _client;

    public QuoteService(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task<bool> CheckMessageAsync(MessageReceivedEventArgs args)
    {
        if (!args.Context.GuildData.Extras.AutoParseQuoteUrls) return false;
        var match = JumpUrl.Match(args.Message.Content);
        if (!match.Success) return false;

        var m = await GetMatchMessageAsync(match);
        if (m is null) return false;
            
        if (m.Content.IsNullOrWhitespace() && m.Embeds.Count != 0) return false;

        await GenerateQuoteEmbed(m, args.Context)
            .SendToAsync(args.Context.Channel)
            .Then(async () =>
            {
                if (match.Groups["Prelink"].Value.IsNullOrEmpty() && match.Groups["Postlink"].Value.IsNullOrEmpty())
                    await args.Message.TryDeleteAsync();
            });
        return true;
    }

    private async Task<RestMessage> GetMatchMessageAsync(Match match)
    {
        if (!match.Groups["GuildId"].Value.TryParse<ulong>(out var guildId) ||
            !match.Groups["ChannelId"].Value.TryParse<ulong>(out var channelId) ||
            !match.Groups["MessageId"].Value.TryParse<ulong>(out var messageId)) return null;

        var g = await _client.Rest.GetGuildAsync(guildId);
        if (g is null) return null;
        var c = await g.GetTextChannelAsync(channelId);
        if (c is null) return null;

        return await c.GetMessageAsync(messageId);
    }

    private static Embed GenerateQuoteEmbed(IMessage message, VolteContext ctx)
    {
        var e = ctx.CreateEmbedBuilder()
            .WithAuthor(message.Author)
            .WithFooter($"Quoted by {ctx.User}", ctx.User.GetEffectiveAvatarUrl());

        if (!message.Content.IsNullOrEmpty())
            e.WithDescription(message.Content);

        if (message.Content.IsNullOrEmpty() && message.HasAttachments())
            e.WithImageUrl(message.Attachments.First().Url);

        if (!message.Content.IsNullOrEmpty() && message.HasAttachments())
            e.WithDescription(message.Content).WithImageUrl(message.Attachments.First().Url);

        e.AddField("Original Message", Format.Url("Click here", message.GetJumpUrl()));

        return e.Build();
    }
    
    private static readonly Regex JumpUrl = JumpUrlPattern();

    [GeneratedRegex(@"(?<Prelink>\S+\s+\S*)?https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<Postlink>\S*\s+\S+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex JumpUrlPattern();
}