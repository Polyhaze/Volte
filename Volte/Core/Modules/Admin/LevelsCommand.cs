using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Levels")]
        public async Task Levels(bool enabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.Leveling = enabled;
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    enabled ? "Enabled Leveling for this server." : "Disabled Leveling for this server."));

        }
    }
}