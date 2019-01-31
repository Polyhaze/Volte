using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Suggest")]
        [Summary("Suggest features for Volte.")]
        [Remarks("Usage: |prefix|suggest")]
        public async Task Suggest() {
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    "You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2)."
                )
            );
        }
    }
}