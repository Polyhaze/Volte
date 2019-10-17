using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels.")]
        [Remarks("tree")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = new StringBuilder().AppendLine(Format.Bold("Uncategorized"));
            var categories = new StringBuilder();

            foreach (var c in Context.Guild.TextChannels
                .Where(c => c.CategoryId == null)
                .Cast<SocketGuildChannel>()
                .Concat(Context.Guild.VoiceChannels
                    .Where(a => a.CategoryId == null)).OrderBy(c => c.Position))
            {
                uncategorized.AppendLine($"- {(c is IVoiceChannel ? "" : "#")}{c.Name}");
            }

            uncategorized.AppendLine();
            foreach (var category in Context.Guild.CategoryChannels.OrderBy(x => x.Position))
            {
                var categoryBuilder = new StringBuilder().AppendLine($"{Format.Bold(category.Name)}");
                foreach (var child in category.Channels.OrderBy(c => c.Position))
                {
                    categoryBuilder.AppendLine($"- {(child is IVoiceChannel ? $"{child.Name}" : $"{child.Cast<ITextChannel>()?.Mention}")}");
                }
                categories.AppendLine(categoryBuilder.ToString());
            }

            var res = uncategorized.AppendLine(categories.ToString()).ToString();

            return res.Length >= 2048 ? BadRequest("This guild is too large; I cannot list all channels here.") : Ok(res);
        }
    }
}
