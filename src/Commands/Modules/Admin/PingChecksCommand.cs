using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("pingchecks {true|false}")]
        [RequireGuildAdmin]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            Context.GuildData.Configuration.Moderation.MassPingChecks = enabled;
            Db.UpdateData(Context.GuildData);
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }
    }
}