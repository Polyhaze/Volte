using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Admin.Configuration {
    public class ServerPrefixCommand : SIVACommand {
        [Command("ServerPrefix")]
        public async Task ServerPrefix([Remainder]string prefix) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.CommandPrefix = prefix;
            await Context.Channel.SendMessageAsync("", false, 
                CreateEmbed(Context,  $"Set this server's prefix to **{prefix}**."));
        }
    }
}