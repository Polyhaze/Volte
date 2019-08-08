using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        [Remarks("Usage: |prefix|feedback {feedback}")]
        public Task<ActionResult> FeedbackAsync([Remainder] string feedback)
        {
            return Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetTextChannel(415182876326232064))
            );
        }
    }
}