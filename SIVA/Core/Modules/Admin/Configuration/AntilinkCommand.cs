using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Admin.Configuration {
    public class AntilinkCommand : SIVACommand {
        [Command("Antilink"), Alias("Al")]
        public async Task Antilink(bool alIsEnabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.Antilink = alIsEnabled;
            ServerConfig.Save();
            var isEnabled = alIsEnabled ? "Antilink has been enabled." : "Antilink has been disabled.";
            await Reply(Context.Channel, CreateEmbed(Context, isEnabled));
        }
    }
}