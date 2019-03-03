using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("DeleteMessageOnCommand", "Dmoc")]
        [Description("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("Usage: |prefix|deletemessageoncommand {true|false}")]
        [RequireGuildAdmin]
        public async Task DeleteMessageOnCommandAsync(bool enabled)
        {
            var config = Db.GetConfig(Context.Guild);
            config.DeleteMessageOnCommand = enabled;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(enabled ? "Enabled DeleteMessageOnCommand in this server." : "Disabled DeleteMessageOnCommand in this server.")
                .SendTo(Context.Channel);
        }
    }
}