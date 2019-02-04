using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("CustomCommandList"), Alias("Ccl")]
        [Summary("Lists custom commands available for this guild.")]
        [Remarks("Usage: |prefix|customcommandlist")]
        public async Task CustomCommandList() {
            var config = Db.GetConfig(Context.Guild);
            var list = string.Empty;
            foreach (var cmd in config.CustomCommands.Keys) {
                list += $"**{cmd}**\n";
            }

            await Reply(Context.Channel, CreateEmbed(Context, list));
        }
    }
}