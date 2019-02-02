using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;
using Volte.Core.Data;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("DeleteMessageOnCommand"), Alias("Dmoc")]
        [Summary("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("Usage: |prefix|deletemessageoncommand {true|false}")]
        public async Task DeleteMessageOnCommand(bool arg) {
            var config = Db.GetConfig(Context.Guild);
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            config.DeleteMessageOnCommand = arg;
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"Set the DeleteMessageOnCommand setting to **{arg}**."));
        }
    }
}