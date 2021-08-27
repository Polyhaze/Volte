using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Discord.Rest;
using Gommon;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    //thanks discord-csharp/MODiX for the idea and some of the code (definitely the regex lol)
    public class QuoteService : IVolteService
    {
        private readonly DiscordShardedClient _client;

        public QuoteService(DiscordShardedClient client) 
            => _client = client;


        private static readonly RegexOptions Options =
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        private static readonly Regex JumpUrlPattern = new Regex(
            @"(?<Prelink>\S+\s+\S*)?https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<Postlink>\S*\s+\S+)?",
            Options);

        public async Task<bool> CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Context.GuildData.Extras.AutoParseQuoteUrls) return false;
            var match = JumpUrlPattern.Match(args.Message.Content);
            if (!match.Success) return false;

            var m = await GetMatchMessageAsync(match);
            if (m is null) return false;
            
            if (m.Content.IsNullOrWhitespace() && !m.Embeds.IsEmpty()) return false;

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
            if (!ulong.TryParse(match.Groups["GuildId"].Value, out var guildId) ||
                !ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId) ||
                !ulong.TryParse(match.Groups["MessageId"].Value, out var messageId)) return null;

            var g = await _client.Rest.GetGuildAsync(guildId);
            if (g is null) return null;
            var c = await g.GetTextChannelAsync(channelId);
            if (c is null) return null;

            return await c.GetMessageAsync(messageId);
        }

        private Embed GenerateQuoteEmbed(IMessage message, VolteContext ctx)
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
    }
}