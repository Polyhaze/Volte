using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Feedback"), Alias("Fb")]
        [Summary("Submit feedback directly to the Volte guild.")]
        [Remarks("Usage: $feedback {feedback}")]
        public async Task Feedback([Remainder]string feedback) {
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Feedback sent! Message: ```{feedback}```"));

            var embed = CreateEmbed(Context, $"```{feedback}```").ToEmbedBuilder()
                .WithTitle($"Feedback from {Context.User.Username}#{Context.User.Discriminator}");
            var channel = VolteBot.Client.GetGuild(405806471578648588).GetTextChannel(415182876326232064);
            await Utils.Send(channel, embed.Build());
        }
    }
}