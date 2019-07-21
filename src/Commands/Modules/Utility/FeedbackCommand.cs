using System.Threading.Tasks;
using Qmmands;
using Volte.Data.Models.Results;
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
            return Ok($"Feedback sent! Message: ```{feedback}```", async _ =>
            {
                await Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(await Context.Client.GetPrimaryGuild().GetTextChannelAsync(415182876326232064));
            });
        }
    }
}