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
    //thanks discord-csharp/MODiX for the idea and some of the code (definitely the regex lol)
    public class QuoteService : VolteEventService
    {
        private readonly DiscordShardedClient _client;

        public QuoteService(DiscordShardedClient client)
        {
            _client = client;
        }

        private static readonly RegexOptions Options =
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        private static readonly Regex JumpUrlPattern = new Regex(
            @"(?<Prelink>\S+\s+\S*)?https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<Postlink>\S*\s+\S+)?",
            Options);
        
        private static readonly Regex JumpUrlRemover = new Regex(
            @"https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(\d+)/(\d+)/(\d+)?",
            Options);

        public override Task DoAsync(EventArgs args)
            => OnMessageReceivedAsync(args.Cast<MessageReceivedEventArgs>() ?? throw new InvalidOperationException($"Quote was triggered with a null event. Expected: {nameof(MessageReceivedEventArgs)}, Received: {args?.GetType().Name}"));

        private async Task OnMessageReceivedAsync(MessageReceivedEventArgs args)
        {
            if (!args.Context.GuildData.Extras.AutoParseQuoteUrls) return;
            var match = JumpUrlPattern.Match(args.Message.Content);
            if (!match.Success) return;
            
            if (!ulong.TryParse(match.Groups["GuildId"].Value, out var guildId) ||
                !ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId) ||
                !ulong.TryParse(match.Groups["MessageId"].Value, out var messageId)) return;

            var g = _client.GetGuild(guildId);
            var c = g?.GetTextChannel(channelId);
                if (c is null) return;

                var m = await c.GetMessageAsync(messageId);
                if (m is null) return;

                await GenerateQuoteEmbed(m, args.Context, match).SendToAsync(args.Context.Channel);

                _ = await args.Message.TryDeleteAsync();
        }

        private Embed GenerateQuoteEmbed(IMessage message, VolteContext ctx, Match match)
        {
            var e = ctx.CreateEmbedBuilder()
                .WithAuthor(message.Author)
                .WithFooter($"Quoted by {ctx.User}", ctx.User.GetAvatarUrl());

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

            if (!match.Groups["Prelink"].Value.IsNullOrEmpty() || !match.Groups["Postlink"].Value.IsNullOrEmpty())
            {
                var strings = Regex.Replace(ctx.Message.Content, JumpUrlRemover.ToString(), " | ")
                    .Split("  ", StringSplitOptions.RemoveEmptyEntries);
                
                if (strings.Length is 2)
                    strings = strings.Select(FilterComments).Where(x => !(x is null)).ToArray();
                e.AddField("Comment", strings.Join(" "), true);
            }

            e.AddField("Original Message", $"[Click here]({message.GetJumpUrl()})");

            return e.Build();
        }
        
        private string FilterComments(string input)
        {
            if (!(input is ""))
            {
                if (input.EndsWith('|') || input.StartsWith('|'))
                    return input.Replace("|", "");
                        
                return input;
            }

            return null;
        }
        
    }
}