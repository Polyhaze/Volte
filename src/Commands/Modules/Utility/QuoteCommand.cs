using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Quote"), Priority(0)]
        [Description("Quotes a user from a given message's ID.")]
        public async Task<ActionResult> QuoteAsync([Description("The ID of the message to quote.")] ulong messageId, [Description("The channel to get the message from. Defaults to the current channel.")] SocketTextChannel channel = null)
        {
            var c = channel ?? Context.Channel;
            var m = await c.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in this channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                .AppendLine($"{m.Content}")
                .AppendLine()
                .AppendLine($"[Jump!]({m.GetJumpUrl()})")
                .ToString())
                .WithAuthor($"{m.Author}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Timestamp.Humanize());
            if (!m.Attachments.IsEmpty())
            {
                e.WithImageUrl(m.Attachments.FirstOrDefault()?.Url);
            }

            return Ok(e);
        }
    }
}