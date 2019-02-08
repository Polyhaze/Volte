using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Say")]
        [Summary("Bot repeats what you tell it to.")]
        [Remarks("Usage: |prefix|say {msg}")]
        public async Task Say([Remainder] string msg) {
            await Context.CreateEmbed(msg).SendTo(Context.Channel);
            await Context.Message.DeleteAsync();
        }
    }
}