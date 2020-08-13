using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("pingchecks {Boolean}")]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.MassPingChecks = enabled;
                return data;
            });
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }
    }
}