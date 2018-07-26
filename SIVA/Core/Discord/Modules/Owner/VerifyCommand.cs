using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class VerifyCommand : SIVACommand {
        [Command("Verify")]
        public async Task Verify(ulong guildId = 0) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            if (guildId == 0) guildId = Context.Guild.Id;

            var config = ServerConfig.Get(SIVA.GetInstance().GetGuild(guildId));

            config.VerifiedGuild = true;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context,
                    $"Successfully verified the guild **{SIVA.GetInstance().GetGuild(guildId).Name}**."));
        }
    }
}