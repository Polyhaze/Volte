using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels.")]
        [Remarks("tree")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = new StringBuilder().AppendLine(Formatter.Bold("Uncategorized"));
            var categories = new StringBuilder();

            foreach (var c in Context.Guild.GetTextChannels()
                .Where(c => c.ParentId == null)
                .Concat(Context.Guild.GetVoiceChannels()
                    .Where(a => a.ParentId == null)).OrderBy(c => c.Position))
            {
                if (CanSeeChannel(Context.Member, c))
                    uncategorized.AppendLine($"- {(c.Type == ChannelType.Voice ? "" : "#")}{c.Name}");
            }

            uncategorized.AppendLine();
            foreach (var category in Context.Guild.GetCategoryChannels().OrderBy(x => x.Position))
            {
                var categoryBuilder = new StringBuilder().AppendLine($"{Formatter.Bold(category.Name)}");
                foreach (var child in category.Children.OrderBy(c => c.Position))
                {
                    categoryBuilder.AppendLine($"- {(child.Type == ChannelType.Voice ? $"{child.Name}" : $"{child.Mention}")}");
                }
                categories.AppendLine(categoryBuilder.ToString());
            }

            var res = uncategorized.AppendLine(categories.ToString()).ToString();

            return res.Length >= 2048 // MaxDescriptionLength is hardcoded in D#+ 
                ? BadRequest("This guild is too large; I cannot list all channels here.") 
                : Ok(res);
        }
    }
}
