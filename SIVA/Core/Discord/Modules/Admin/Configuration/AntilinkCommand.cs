using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class AntilinkCommand : SIVACommand {
        [Command("Antilink"), Alias("Al")]
        public async Task Antilink(bool alIsEnabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.Message, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.Antilink = alIsEnabled;
            ServerConfig.Save();
            var isEnabled = alIsEnabled ? "Antilink has been enabled." : "Antilink has been disabled.";
            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, isEnabled));
        }
    }
}