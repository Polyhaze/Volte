using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Owner {
    public class CreateConfigCommand : SIVACommand {
        [Command("CreateConfig")]
        public async Task CreateConfig(ulong serverId = 0) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            if (serverId == 0) serverId = Context.Guild.Id;

            var tG = Discord.SIVA.Client.GetGuild(serverId);

            ServerConfig.Get(tG);
            ServerConfig.Save();
        }
    }
}