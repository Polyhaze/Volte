using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.General
{
    public partial class GeneralModule : VolteModule
    {
        [Command("Say")]
        [Description("Bot repeats what you tell it to.")]
        [Remarks("Usage: |prefix|say {msg}")]
        public async Task Say([Remainder] string msg)
        {
            await Context.CreateEmbed(msg).SendTo(Context.Channel);
            await Context.Message.DeleteAsync();
        }

        [Command("SilentSay")]
        [Description("Runs the say command normally, but doesn't show the author in the embed.")]
        [Remarks("Usage: |prefix|silentsay {msg}")]
        public async Task SilentSay([Remainder] string msg)
        {
            await new EmbedBuilder()
                .WithColor(Config.GetSuccessColor())
                .WithDescription(msg)
                .SendTo(Context.Channel);
            await Context.Message.DeleteAsync();
        }
    }
}