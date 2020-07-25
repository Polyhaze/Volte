using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Gommon;
using Volte.Commands;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    //thanks MODiX for the idea and some of the code (definitely the regex lol)
    public class QuoteService : VolteEventService
    {
        private readonly DiscordShardedClient _client;

        public QuoteService(DiscordShardedClient client)
        {
            _client = client;
        }

        private static readonly Regex JumpUrlPattern = new Regex(
            @"(?<Prelink>\S+\s+\S*)?https?://(?:(?:ptb|canary)\.)?discordapp\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<Postlink>\S*\s+\S+)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public override Task DoAsync(EventArgs args)
            => OnMessageReceivedAsync(args.Cast<MessageReceivedEventArgs>());

        private async Task OnMessageReceivedAsync(MessageReceivedEventArgs args)
        {
            if (!args.Context.GuildData.Extras.AutoParseQuoteUrls) return;
            foreach (Match match in JumpUrlPattern.Matches(args.Message.Content))
            {
                if (!ulong.TryParse(match.Groups["GuildId"].Value, out _) ||
                    !ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId) ||
                    !ulong.TryParse(match.Groups["MessageId"].Value, out var messageId)) continue;

                var c = _client.GetChannel(channelId);
                if (!(c is ITextChannel channel)) continue;

                var m = await channel.GetMessageAsync(messageId);
                if (m is null) return;

                await GenerateQuoteEmbed(m, args.Context, match).SendToAsync(args.Context.Channel);

                _ = await args.Message.TryDeleteAsync();
            }
        }

        private Embed GenerateQuoteEmbed(IMessage message, VolteContext ctx, Match match)
        {
            var e = ctx.CreateEmbedBuilder()
                .WithAuthor(message.Author)
                .AddField("Quoted By", $"**{ctx.User}** in {ctx.Channel.Mention}");
            if (!message.Content.IsNullOrEmpty())
            {
                e.WithDescription(message.Content);
            }

            if (message.Content.IsNullOrEmpty() && message.HasAttachments())
            {
                e.WithImageUrl(message.Attachments.First().Url);
            }

            if (!message.Content.IsNullOrEmpty() && message.HasAttachments())
            {
                e.WithDescription(message.Content).WithImageUrl(message.Attachments.First().Url);
            }

            if (!match.Groups["Prelink"].Value.IsNullOrEmpty())
            {
                e.AddField("Comment", match.Groups["Prelink"].Value);
            }

            if (!match.Groups["Postlink"].Value.IsNullOrEmpty())
            {
                e.AddField("Comment", match.Groups["Postlink"].Value);
            }

            return e.Build();
        }
        
    }
}