using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin.Configuration {
    public class ServerPrefixCommand : VolteCommand {
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