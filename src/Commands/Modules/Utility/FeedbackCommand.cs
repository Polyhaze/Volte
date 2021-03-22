using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        public Task<ActionResult> FeedbackAsync([Remainder, Description("The feedback you want to submit for me. Please make it constructive.")] string feedback)
            => Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetTextChannel(415182876326232064))
            );
    }
}