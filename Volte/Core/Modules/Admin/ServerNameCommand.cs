using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("ServerName")]
        [Summary("Sets the name of this guild.")]
        [Remarks("Usage: |prefix|servername {newName}")]
        public async Task ServerName([Remainder] string name) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await Context.Guild.ModifyAsync(g => g.Name = name);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Set this server's name to **{name}**."));
        }
    }
}