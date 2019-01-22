using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Owner {
    public class ForceLeaveCommand : SIVACommand {
        [Command("ForceLeave")]
        public async Task ForceLeave([Remainder]string serverName) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var target = Discord.SIVA.Client.Guilds.FirstOrDefault(g => g.Name == serverName);
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