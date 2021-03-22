using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        public Task<ActionResult> AntilinkAsync(bool enabled)
        {
            Context.GuildData.Configuration.Moderation.Antilink = enabled;
            Db.Save(Context.GuildData);
            return Ok(enabled ? "Antilink has been enabled." : "Antilink has been disabled.");
        }
    }
}