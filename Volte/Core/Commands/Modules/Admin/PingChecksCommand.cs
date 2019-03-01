using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("Usage: |prefix|pingchecks {true|false}")]
        [RequireGuildAdmin]
        public async Task PingChecksAsync(bool enabled)
        {
            var config = Db.GetConfig(Context.Guild);
            config.ModerationOptions.MassPingChecks = enabled;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.")
                .SendTo(Context.Channel);
        }
    }
}