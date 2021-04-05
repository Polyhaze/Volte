using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels.")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = Context.CreateEmbedBuilder();
            var categories = new List<EmbedBuilder>();
            var toIterate = Context.Guild.TextChannels.Where(c => c.CategoryId == null)
                .Cast<SocketGuildChannel>()
                .Concat(Context.Guild.VoiceChannels.Where(a => a.CategoryId == null))
                .OrderBy(c => c.Position).ToList();

            if (toIterate.IsEmpty())
            {
                uncategorized = null;
            }
            else
            {
                foreach (var c in toIterate)
                {
                    uncategorized.WithDescription(uncategorized.Description + $"- {(c is IVoiceChannel ? c.Name : c.Cast<ITextChannel>()?.Mention)}\n");
                }
            }

            //uncategorized.AppendLine();
            foreach (var category in Context.Guild.CategoryChannels.OrderBy(x => x.Position))
            {
                var embedBuilder = Context.CreateEmbedBuilder().WithTitle(category.Name);
                var textChannels = category.Channels.Where(c => c is ITextChannel)
                    .OrderBy(c => c.Position).ToList()
                    .Select(x => x.Cast<ITextChannel>()).ToArray();
                foreach (var child in textChannels)
                {
                    embedBuilder.WithDescription((embedBuilder.Description ?? "") + $"- {child.Mention}\n");
                    //categoryBuilder.AppendLine($"- {child.Mention}");
                }

                var voiceChannels = category.Channels.Where(c => c is IVoiceChannel)
                    .OrderBy(c => c.Position).ToList()
                    .Select(x => x.Cast<IVoiceChannel>()).ToArray();
                
                foreach (var channel in voiceChannels)
                {
                    embedBuilder.WithDescription((embedBuilder.Description ?? "") + $"- {channel.Name}\n");
                    //categoryBuilder.AppendLine($"- {channel.Name}");
                }
                categories.Add(embedBuilder);
                //categories.AppendLine(categoryBuilder.ToString());
            }

            //var res = uncategorized.AppendLine(categories.ToString()).ToString();
            var res = new List<EmbedBuilder>();
            if (uncategorized != null)
            {
                res.Add(uncategorized);   
            }

            if (!categories.IsEmpty())
            {
                res.AddRange(categories);
            }

            return res.Count is 1 ? Ok(res[0]) : Ok(res);

            //return res.Length >= 2048 ? BadRequest("This guild is too large; I cannot show the tree of channels here.") : Ok(res);
        }
    }
}
