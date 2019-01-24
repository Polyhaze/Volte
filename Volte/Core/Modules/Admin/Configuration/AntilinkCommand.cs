using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin.Configuration {
    public class AntilinkCommand : VolteCommand {
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