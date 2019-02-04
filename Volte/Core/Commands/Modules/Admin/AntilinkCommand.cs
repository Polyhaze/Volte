using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Antilink"), Alias("Al")]
        [Summary("Enable/Disable Antilink for the current guild.")]
        [Remarks("Usage: |prefix|antilink {true|false}")]
        [RequireGuildAdmin]
        public async Task Antilink(bool alIsEnabled) {
            var config = Db.GetConfig(Context.Guild);
            config.Antilink = alIsEnabled;
            var isEnabled = alIsEnabled ? "Antilink has been enabled." : "Antilink has been disabled.";
            await Reply(Context.Channel, CreateEmbed(Context, isEnabled));
        }
    }
}