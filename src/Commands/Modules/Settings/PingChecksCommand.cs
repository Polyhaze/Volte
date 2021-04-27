using System.Threading.Tasks;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("PingChecks", "MassMentions")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            Context.Modify(data => data.Configuration.Moderation.MassPingChecks = enabled);
            return Ok(enabled ? "Checking for excessive mentions has been enabled." : "Checking for excessive mentions has been disabled.");
        }
    }
}