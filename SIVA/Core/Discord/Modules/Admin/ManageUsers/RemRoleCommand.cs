using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.ManageUsers {
    public class RemRoleCommand : SivaCommand {
        [Command("RemRole"), Alias("Rr")]
        public async Task RemRole(SocketGuildUser user, [Remainder] string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == role.ToLower());
            if (targetRole != null) {
                await user.RemoveRoleAsync(targetRole);
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"Removed the role **{role}** from {user.Mention}!"));
                return;
            }

            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"**{role}** doesn't exist on this server!"));
        }
    }
}