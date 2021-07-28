using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Tree")]
        [Description("Shows all categories in this guild and their children channels in a paginator.")]
        public Task<ActionResult> TreeAsync()
        {
            var uncategorized = Context.CreateEmbedBuilder();
            var categories = new List<EmbedBuilder>();
            var toIterate = Context.Guild.TextChannels.Where(c => c.CategoryId is null)
                .Cast<SocketGuildChannel>()
                .Concat(Context.Guild.VoiceChannels.Where(a => a.CategoryId is null))
                .OrderBy(c => c.Position).ToList();

            if (toIterate.IsEmpty())
                uncategorized = null;
            else
                foreach (var c in toIterate)
                    uncategorized.AppendDescriptionLine(
                        $"- {(c is IVoiceChannel ? c.Name : c.Cast<ITextChannel>()!.Mention)}");
            
            foreach (var category in Context.Guild.CategoryChannels.OrderBy(x => x.Position))
            {
                var embedBuilder = Context.CreateEmbedBuilder().WithTitle(category.Name);
                var textChannels = category.Channels.OfType<ITextChannel>()
                    .OrderBy(c => c.Position).ToArray();
                foreach (var child in textChannels)
                    embedBuilder.AppendDescriptionLine($"- {child.Mention}");

                var voiceChannels = category.Channels.OfType<IVoiceChannel>()
                    .OrderBy(c => c.Position).ToArray();

                foreach (var channel in voiceChannels)
                    embedBuilder.AppendDescriptionLine($"- {channel.Name}");

                categories.Add(embedBuilder);
            }

            var res = new List<EmbedBuilder>();
            if (uncategorized != null)
                res.Add(uncategorized.WithTitle("Uncategorized"));

            if (!categories.IsEmpty())
                res.AddRange(categories);

            return res.Count is 1 ? Ok(res[0]) : Ok(res);
        }
    }
}