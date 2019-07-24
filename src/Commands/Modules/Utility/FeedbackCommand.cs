using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        [Remarks("Usage: |prefix|feedback {feedback}")]
        public Task<VolteCommandResult> FeedbackAsync([Remainder] string feedback)
        {
            return Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetTextChannel(415182876326232064))
            );
        }
    }
}