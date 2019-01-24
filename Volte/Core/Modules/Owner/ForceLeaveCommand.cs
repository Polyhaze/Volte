using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public class ForceLeaveCommand : VolteCommand {
        [Command("ForceLeave")]
        public async Task ForceLeave([Remainder]string serverName) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var target = VolteBot.Client.Guilds.FirstOrDefault(g => g.Name == serverName);
            if (target == null) {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"I'm not in the guild **{serverName}**."));
                return;
            }

            await target.LeaveAsync();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Successfully left {target.Name}"));
        }
    }
}