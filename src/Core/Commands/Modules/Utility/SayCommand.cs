using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("Usage: |prefix|say {msg}")]
        public async Task SayAsync([Remainder] string msg)
        {
            await Context.CreateEmbed(msg).SendTo(Context.Channel);
            await Context.Message.DeleteAsync();
        }

        [Command("SilentSay")]
        [Description("Runs the say command normally, but doesn't show the author in the message.")]
        [Remarks("Usage: |prefix|silentsay {msg}")]
        public async Task SilentSayAsync([Remainder] string msg)
        {
            await new EmbedBuilder()
                .WithColor(Config.SuccessColor)
                .WithDescription(msg)
                .SendTo(Context.Channel);
            await Context.Message.DeleteAsync();
        }
    }
}