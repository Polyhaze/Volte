using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("PingChecks")]
        [Summary("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("Usage: |prefix|pingchecks {true|false}")]
        [RequireGuildAdmin]
        public async Task PingChecks(bool arg) {
            var config = Db.GetConfig(Context.Guild);
            config.MassPingChecks = arg;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(arg ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.")
                .SendTo(Context.Channel);
        }
    }
}