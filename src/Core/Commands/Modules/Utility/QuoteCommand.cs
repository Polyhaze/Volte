using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Quote"), Priority(0)]
        [Description("Quotes a user from a given message's ID.")]
        [Remarks("Usage: |prefix|quote {messageId}")]
        public async Task QuoteAsync(ulong messageId)
        {
            var m = await Context.Channel.GetMessageAsync(messageId);
            if (m is null)
            {
                await Context.CreateEmbed("A message with that ID doesn't exist in this channel.")
                    .SendTo(Context.Channel);
                return;
            }
            await Context.CreateEmbedBuilder($"{m.Content}\n\n[Jump!]({m.GetJumpUrl()})")
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Timestamp.Humanize())
                .SendTo(Context.Channel);
        }

        [Command("Quote"), Priority(1)]
        [Description("Quotes a user in a different chanel from a given message's ID.")]
        [Remarks("Usage: |prefix|quote {messageId}")]
        public async Task QuoteAsync(SocketTextChannel channel, ulong messageId)
        {
            var m = await channel.GetMessageAsync(messageId);
            if (m is null)
            {
                await Context.CreateEmbed("A message with that ID doesn't exist in the given channel.")
                    .SendTo(Context.Channel);
                return;
            }
            await Context.CreateEmbedBuilder($"{m.Content}\n\n[Jump!]({m.GetJumpUrl()})")
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Timestamp.Humanize())
                .SendTo(Context.Channel);
        }
    }
}
