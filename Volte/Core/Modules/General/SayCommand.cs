using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Say")]
        [Summary("Bot repeats what you tell it to.")]
        [Remarks("Usage: |prefix|say {msg}")]
        public async Task Say([Remainder] string msg) {
            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, msg));
        }
    }
}