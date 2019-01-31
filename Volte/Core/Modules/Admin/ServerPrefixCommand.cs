using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("ServerPrefix")]
        [Summary("Sets the command prefix for this guild.")]
        [Remarks("Usage: |prefix|serverprefix {newPrefix}")]
        public async Task ServerPrefix([Remainder]string prefix) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.CommandPrefix = prefix;
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context,  $"Set this server's prefix to **{prefix}**."));
        }
    }
}