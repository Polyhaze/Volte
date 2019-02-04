using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Levels")]
        [Summary("Enables/Disables level gaining for this guild.")]
        [Remarks("Usage: $levels {true|false}")]
        [RequireGuildAdmin]
        public async Task Levels(bool enabled) {
            var config = Db.GetConfig(Context.Guild);
            config.Leveling = enabled;
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context,
                    enabled ? "Enabled Leveling for this server." : "Disabled Leveling for this server."));

        }
    }
}