using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin.Server {
    public class ServerNameCommand : VolteCommand {
        [Command("ServerName")]
        public async Task ServerName([Remainder] string name) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await Context.Guild.ModifyAsync(g => g.Name = name);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set this server's name to **{name}**."));
        }
    }
}