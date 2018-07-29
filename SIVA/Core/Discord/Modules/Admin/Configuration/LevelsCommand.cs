using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class LevelsCommand : SIVACommand {
        [Command("Levels")]
        public async Task Levels(bool enabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }
            var config = ServerConfig.Get(Context.Guild);
            config.Leveling = enabled;
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context,
                    enabled ? "Enabled Leveling for this server." : "Disabled Leveling for this server."));

        }
    }
}