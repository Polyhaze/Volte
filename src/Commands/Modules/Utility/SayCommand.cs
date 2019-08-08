using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("Usage: |prefix|say {msg}")]
        public Task<ActionResult> SayAsync([Remainder] string msg)
        {
            return Ok(msg, _ => Context.Message.DeleteAsync());
        }

        [Command("SilentSay", "Ssay")]
        [Description("Runs the say command normally, but doesn't show the author in the message.")]
        [Remarks("Usage: |prefix|silentsay {msg}")]
        public Task<ActionResult> SilentSayAsync([Remainder] string msg)
        {
            return Ok(new EmbedBuilder()
                .WithColor(Config.SuccessColor)
                .WithDescription(msg), _ => Context.Message.DeleteAsync());
        }
    }
}