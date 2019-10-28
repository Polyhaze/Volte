using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("antilink {true|false}")]
        [RequireGuildAdmin]
        public Task<ActionResult> AntilinkAsync(bool enabled)
        {
            Context.GuildData.Configuration.Moderation.Antilink = enabled;
            Db.UpdateData(Context.GuildData);
            return Ok(enabled ? "Antilink has been enabled." : "Antilink has been disabled.");
        }
    }
}