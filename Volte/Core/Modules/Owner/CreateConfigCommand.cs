using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public class CreateConfigCommand : VolteCommand {
        [Command("CreateConfig")]
        public async Task CreateConfig(ulong serverId = 0) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            if (serverId == 0) serverId = Context.Guild.Id;

            var tG = VolteBot.Client.GetGuild(serverId);

            ServerConfig.Get(tG);
            ServerConfig.Save();
        }
    }
}