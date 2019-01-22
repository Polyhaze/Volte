using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Admin.ManageUsers {
    public class AddRoleCommand : SIVACommand {
        [Command("AddRole"), Alias("Ar")]
        public async Task AddRole(SocketGuildUser user, [Remainder] string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == role.ToLower());
            if (targetRole != null) {
                await user.AddRoleAsync(targetRole);
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"Added the role **{role}** to {user.Mention}!"));
                return;
            }

            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"**{role}** doesn't exist on this server!"));
        }
    }
}