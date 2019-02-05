using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Feedback"), Alias("Fb")]
        [Summary("Submit feedback directly to the Volte guild.")]
        [Remarks("Usage: $feedback {feedback}")]
        public async Task Feedback([Remainder]string feedback) {
            await Context.CreateEmbed($"Feedback sent! Message: ```{feedback}```").SendTo(Context.Channel);

            var embed = Context.CreateEmbed($"```{feedback}```").ToEmbedBuilder()
                .WithTitle($"Feedback from {Context.User.Username}#{Context.User.Discriminator}");
            await embed.SendTo(VolteBot.Client.GetGuild(405806471578648588).GetTextChannel(415182876326232064));
        }
    }
}