using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("PengChecks")]
        public async Task PengChecks(bool isEnabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.MassPengChecks = true;
            Db.UpdateConfig(config);

            var pcIsEnabled = isEnabled ? "Enabled mass ping checks." : "Disabled mass ping checks.";
            await Reply(Context.Channel, CreateEmbed(Context, pcIsEnabled));
        }
    }
}