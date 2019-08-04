using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("DeleteMessageOnCommand", "Dmoc")]
        [Description("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("Usage: |prefix|deletemessageoncommand {true|false}")]
        [RequireGuildAdmin]
        public Task<ActionResult> DeleteMessageOnCommandAsync(bool enabled)
        {
            Context.GuildData.Configuration.DeleteMessageOnCommand = enabled;
            Db.UpdateData(Context.GuildData);
            return Ok(enabled
                ? "Enabled DeleteMessageOnCommand in this server."
                : "Disabled DeleteMessageOnCommand in this server.");
        }
    }
}