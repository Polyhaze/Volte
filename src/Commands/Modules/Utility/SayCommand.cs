using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Core;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("say {String}")]
        public Task<ActionResult> SayAsync([Remainder]string msg) 
            => Ok(msg, _ => Context.Message.DeleteAsync());

        [Command("SilentSay", "SSay")]
        [Description("Runs the say command normally, but doesn't show the author in the message. Useful for announcements.")]
        [Remarks("silentsay {String}")]
        public Task<ActionResult> SilentSayAsync([Remainder]string msg) 
            => Ok(new DiscordEmbedBuilder()
                .WithColor(Config.SuccessColor)
                .WithDescription(msg), _ => Context.Message.DeleteAsync());
    }
}