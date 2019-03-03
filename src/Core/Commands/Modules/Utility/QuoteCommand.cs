using System.Threading.Tasks;
using Discord;
using Humanizer;
using Qmmands;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Quote")]
        [Description("Quotes a user from a given message's ID.")]
        [Remarks("Usage: |prefix|quote {messageId}")]
        public async Task QuoteAsync(ulong messageId)
        {
            var m = await Context.Channel.GetMessageAsync(messageId);
            await new EmbedBuilder()
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithColor(Config.SuccessColor)
                .WithDescription($"{m.Content}\n\n[Jump!]({m.GetJumpUrl()})")
                .WithFooter(m.Timestamp.Humanize())
                .SendTo(Context.Channel);
        }

    }
}
