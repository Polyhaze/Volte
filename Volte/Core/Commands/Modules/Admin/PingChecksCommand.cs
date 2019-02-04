using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("PingChecks")]
        [Summary("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("Usage: |prefix|pingchecks {true|false}")]
        [RequireGuildAdmin]
        public async Task PingChecks(bool isEnabled) {
            var config = Db.GetConfig(Context.Guild);
            config.MassPingChecks = true;
            Db.UpdateConfig(config);

            var pcIsEnabled = isEnabled ? "Enabled mass ping checks." : "Disabled mass ping checks.";
            await Reply(Context.Channel, CreateEmbed(Context, pcIsEnabled));
        }
    }
}