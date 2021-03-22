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
            Context.GuildData.Configuration.Moderation.MassPingChecks = enabled;
            Db.Save(Context.GuildData);
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }
    }
}