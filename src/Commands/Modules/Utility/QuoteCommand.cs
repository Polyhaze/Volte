using System.Threading.Tasks;
using DSharpPlus.Entities;
using Humanizer;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
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
                    .SendToAsync(Context.Channel);
                return;
            }
            await Context.CreateEmbedBuilder($"{m.Content}\n\n[Jump!]({m.JumpLink})")
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}, in #{m.Channel.Name}",
                    m.Author.AvatarUrl)
                .WithFooter(m.Timestamp.Humanize())
                .SendToAsync(Context.Channel);
        }

        [Command("Quote"), Priority(1)]
        [Description("Quotes a user in a different chanel from a given message's ID.")]
        [Remarks("Usage: |prefix|quote {messageId}")]
        public async Task QuoteAsync(DiscordChannel channel, ulong messageId)
        {
            var m = await channel.GetMessageAsync(messageId);
            if (m is null)
            {
                await Context.CreateEmbed("A message with that ID doesn't exist in the given channel.")
                    .SendToAsync(Context.Channel);
                return;
            }
            await Context.CreateEmbedBuilder($"{m.Content}\n\n[Jump!]({m.JumpLink})")
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}, in #{m.Channel.Name}",
                    m.Author.AvatarUrl)
                .WithFooter(m.Timestamp.Humanize())
                .SendToAsync(Context.Channel);
        }
    }
}
