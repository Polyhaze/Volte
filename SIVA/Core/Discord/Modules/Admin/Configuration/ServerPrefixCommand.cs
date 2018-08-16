using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class ServerPrefixCommand : SIVACommand {
        public async Task ServerPrefix([Remainder]string prefix) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.CommandPrefix = prefix;
            await Context.Channel.SendMessageAsync("", false, Utils.CreateEmbed(Context, 
                $"Set this server's prefix to **{prefix}**."));
        }
    }
}