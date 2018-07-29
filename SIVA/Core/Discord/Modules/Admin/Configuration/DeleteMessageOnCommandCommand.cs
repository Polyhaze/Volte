using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Helpers;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class DeleteMessageOnCommandCommand : SIVACommand {
        [Command("DeleteMessageOnCommand"), Alias("Dmoc")]
        public async Task DeleteMessageOnCommand(bool arg) {
            var config = ServerConfig.Get(Context.Guild);
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
            }

            config.DeleteMessageOnCommand = arg;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the DeleteMessageOnCommand setting to {arg}."));
        }
    }
}