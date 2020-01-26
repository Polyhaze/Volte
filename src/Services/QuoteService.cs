using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Gommon;
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
                if (ulong.TryParse(match.Groups["GuildId"].Value, out _)
                    && ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId)
                    && ulong.TryParse(match.Groups["MessageId"].Value, out var messageId))
                {
                    var c = _client.GetChannel(channelId);
                    if (c is ITextChannel channel)
                    {
                        var m = await channel.GetMessageAsync(messageId);
                        if (m is null) return;
                        await args.Context.CreateEmbedBuilder()
                            .WithAuthor(m.Author)
                            .WithDescription(Format.Code(m.Content))
                            .AddField("Quoted By", $"**{args.Context.User}** in {args.Context.Channel.Mention}")
                            .SendToAsync(args.Context.Channel);

                        if (match.Groups["Prelink"].Value.IsNullOrEmpty() &&
                            match.Groups["Postlink"].Value.IsNullOrEmpty())
                            _ = await args.Context.Message.TryDeleteAsync();
                    }
                }
            }
        }
    }
}