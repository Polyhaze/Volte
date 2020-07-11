using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Quote"), Priority(0)]
        [Description("Quotes a user from a given message's ID.")]
        [Remarks("quote {Ulong}")]
        public async Task<ActionResult> QuoteAsync(ulong messageId)
        {
            var m = await Context.Channel.GetMessageAsync(messageId);
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
            if (m.Attachments.Count > 0)
            {
                e.WithImageUrl(m.Attachments.FirstOrDefault()?.Url);
            }

            return Ok(e);
        }

        [Command("Quote"), Priority(1)]
        [Description("Quotes a user in a different chanel from a given message's ID.")]
        [Remarks("quote {messageId}")]
        public async Task<ActionResult> QuoteAsync(SocketTextChannel channel, ulong messageId)
        {
            var m = await channel.GetMessageAsync(messageId);
            if (m is null)
                return BadRequest("A message with that ID doesn't exist in the given channel.");

            var e = Context.CreateEmbedBuilder(new StringBuilder()
                    .AppendLine($"{m.Content}")
                    .AppendLine()
                    .AppendLine($"[Jump!]({m.GetJumpUrl()})")
                    .ToString())
                .WithAuthor($"{m.Author}, in #{m.Channel.Name}",
                    m.Author.GetAvatarUrl())
                .WithFooter(m.Timestamp.Humanize());
            if (m.Attachments.Count > 0)
            {
                e.WithImageUrl(m.Attachments.FirstOrDefault()?.Url);
            }

            return Ok(e);
        }
    }
}