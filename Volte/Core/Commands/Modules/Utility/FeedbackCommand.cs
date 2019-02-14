using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        [Remarks("Usage: $feedback {feedback}")]
        public async Task Feedback([Remainder] string feedback)
        {
            await Context.CreateEmbed($"Feedback sent! Message: ```{feedback}```").SendTo(Context.Channel);

            var embed = Context.CreateEmbed($"```{feedback}```").ToEmbedBuilder()
                .WithTitle($"Feedback from {Context.User.Username}#{Context.User.Discriminator}");
            await embed.SendTo(VolteBot.Client.GetGuild(405806471578648588).GetTextChannel(415182876326232064));
        }
    }
}