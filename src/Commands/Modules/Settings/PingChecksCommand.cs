using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            Context.Modify(data => data.Configuration.Moderation.MassPingChecks = enabled);
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }
    }
}