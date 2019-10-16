using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("say {msg}")]
        public Task<ActionResult> SayAsync([Remainder] string msg) 
            => Ok(msg, _ => Context.Message.DeleteAsync());

        [Command("SilentSay", "Ssay")]
        [Description("Runs the say command normally, but doesn't show the author in the message.")]
        [Remarks("silentsay {msg}")]
        public Task<ActionResult> SilentSayAsync([Remainder] string msg) 
            => Ok(new EmbedBuilder()
                .WithColor(Config.SuccessColor)
                .WithDescription(msg), _ => Context.Message.DeleteAsync());
    }
}