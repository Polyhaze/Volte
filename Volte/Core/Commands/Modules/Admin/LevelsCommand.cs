using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Levels")]
        [Summary("Enables/Disables level gaining for this guild.")]
        [Remarks("Usage: $levels {true|false}")]
        [RequireGuildAdmin]
        public async Task Levels(bool arg) {
            var config = Db.GetConfig(Context.Guild);
            config.Leveling = arg;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(arg ? "Leveling has been enabled." : "Leveling has been disabled.")
                .SendTo(Context.Channel);

        }
    }
}