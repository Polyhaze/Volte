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
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels.")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = new StringBuilder();
            var categories = new StringBuilder();

            foreach (var c in Context.Guild.TextChannels
                .Where(c => c.CategoryId == null)
                .Cast<SocketGuildChannel>()
                .Concat(Context.Guild.VoiceChannels
                    .Where(a => a.CategoryId == null)).OrderBy(c => c.Position))
            {
                uncategorized.AppendLine($"- {(c is IVoiceChannel ? c.Name : c.Cast<ITextChannel>()?.Mention)}");
            }

            uncategorized.AppendLine();
            foreach (var category in Context.Guild.CategoryChannels.OrderBy(x => x.Position))
            {
                var index = 0;
                var text = 0;
                var voice = 0;
                var categoryBuilder = new StringBuilder().AppendLine($"{Format.Bold(category.Name)}");
                var textChannels = category.Channels.Where(c => c is ITextChannel)
                    .OrderBy(c => c.Position).ToList()
                    .Select(x => x.Cast<ITextChannel>()).ToArray();
                foreach (var child in textChannels)
                {
                    if (index >= 5)                     {
                        categoryBuilder.AppendLine(Format.Bold($"And {"other text channel".ToQuantity(textChannels.Length - text)}."));
                        break;
                    }
                    categoryBuilder.AppendLine($"- {child.Mention}");
                    text++;
                    index++;
                }

                var voiceChannels = category.Channels.Where(c => c is IVoiceChannel)
                    .OrderBy(c => c.Position).ToList()
                    .Select(x => x.Cast<IVoiceChannel>()).ToArray();
                
                foreach (var channel in voiceChannels)
                {
                    if (index >= 5)
                    {
                        categoryBuilder.AppendLine(Format.Bold($"And {"other voice channel".ToQuantity(voiceChannels.Length - voice)}."));
                        break;
                    }
                    categoryBuilder.AppendLine($"- {channel.Name}");
                    voice++;
                    index++;
                }
                categories.AppendLine(categoryBuilder.ToString());
            }

            var res = uncategorized.AppendLine(categories.ToString()).ToString();

            return res.Length >= 2048 ? BadRequest("This guild is too large; I cannot show the tree of channels here.") : Ok(res);
        }
    }
}
