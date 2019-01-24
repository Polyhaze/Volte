using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public class SuggestCommand : VolteCommand {
        [Command("Suggest")]
        public async Task Suggest() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(
                    Context,
                    "You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2)."
                )
            );
        }
    }
}