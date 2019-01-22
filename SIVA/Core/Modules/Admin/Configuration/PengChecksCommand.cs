using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Admin.Configuration {
    public class PengChecksCommand : SIVACommand {
        [Command("PengChecks")]
        public async Task PengChecks(bool isEnabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.MassPengChecks = true;
            ServerConfig.Save();

            var pcIsEnabled = isEnabled ? "Enabled mass ping checks." : "Disabled mass ping checks.";
            await Reply(Context.Channel, CreateEmbed(Context, pcIsEnabled));
        }
    }
}