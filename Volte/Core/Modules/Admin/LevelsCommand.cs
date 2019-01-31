using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Levels")]
        [Summary("Enables/Disables level gaining for this guild.")]
        [Remarks("Usage: $levels {true|false}")]
        public async Task Levels(bool enabled) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.Leveling = enabled;
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context,
                    enabled ? "Enabled Leveling for this server." : "Disabled Leveling for this server."));

        }
    }
}