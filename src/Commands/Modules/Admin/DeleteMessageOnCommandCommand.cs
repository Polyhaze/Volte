using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("DeleteMessageOnCommand", "Dmoc")]
        [Description("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("Usage: |prefix|deletemessageoncommand {true|false}")]
        [RequireGuildAdmin]
        public async Task DeleteMessageOnCommandAsync(bool enabled)
        {
            var data = Db.GetData(Context.Guild);
            config.DeleteMessageOnCommand = enabled;
            Db.UpdateData(config);
            await Context.CreateEmbed(enabled
                    ? "Enabled DeleteMessageOnCommand in this server."
                    : "Disabled DeleteMessageOnCommand in this server.")
                .SendToAsync(Context.Channel);
        }
    }
}