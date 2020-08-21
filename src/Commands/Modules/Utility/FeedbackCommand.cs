using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        [Remarks("feedback {String}")]
        public Task<ActionResult> FeedbackAsync([Remainder]string feedback)
            => Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.Member}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetChannel(415182876326232064))
            );
    }
}