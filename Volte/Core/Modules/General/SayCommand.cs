using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public class SayCommand : VolteCommand {
        [Command("Say")]
        public async Task Say([Remainder] string msg) {
            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, msg));
        }
    }
}