using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class CreateConfigCommand : SivaCommand {
        [Command("CreateConfig")]
        public async Task CreateConfig(ulong serverId = 0) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            if (serverId == 0) serverId = Context.Guild.Id;

            var tG = Siva.GetInstance().GetGuild(serverId);

            ServerConfig.Get(tG);
            ServerConfig.Save();
        }
    }
}