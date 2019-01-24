using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Helpers;
using Volte.Core.Files.Readers;

namespace Volte.Core.Modules.Admin.Configuration {
    public class DeleteMessageOnCommandCommand : VolteCommand {
        [Command("DeleteMessageOnCommand"), Alias("Dmoc")]
        public async Task DeleteMessageOnCommand(bool arg) {
            var config = ServerConfig.Get(Context.Guild);
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
            }

            config.DeleteMessageOnCommand = arg;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set the DeleteMessageOnCommand setting to {arg}."));
        }
    }
}