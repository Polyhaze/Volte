using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Interaction;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        public Task<ActionResult> FeedbackAsync(
            [Remainder, Description("The feedback you want to submit for me. Please make it constructive.")]
            string feedback)
            => Ok($"Feedback sent! Message: {Format.Code(feedback, string.Empty)}", _ =>
                Context.CreateEmbedBuilder($"{Format.Code(feedback, string.Empty)}")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(Context.Client.GetPrimaryGuild().GetTextChannel(415182876326232064))
            );
    }
}