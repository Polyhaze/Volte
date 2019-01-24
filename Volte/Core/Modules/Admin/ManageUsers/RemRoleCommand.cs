using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin.ManageUsers {
    public class RemRoleCommand : VolteCommand {
        [Command("RemRole"), Alias("Rr")]
        public async Task RemRole(SocketGuildUser user, [Remainder] string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == role.ToLower());
            if (targetRole != null) {
                await user.RemoveRoleAsync(targetRole);
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"Removed the role **{role}** from {user.Mention}!"));
                return;
            }

            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"**{role}** doesn't exist on this server!"));
        }
    }
}