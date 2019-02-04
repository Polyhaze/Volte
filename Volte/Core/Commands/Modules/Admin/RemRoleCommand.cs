using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("RemRole"), Alias("Rr")]
        [Summary("Remove a role from the mentioned user.")]
        [Remarks("Usage: |prefix|remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task RemRole(SocketGuildUser user, [Remainder] string role) {
            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == role.ToLower());
            if (targetRole != null) {
                await user.RemoveRoleAsync(targetRole);
                await Context.Channel.SendMessageAsync(string.Empty, false,
                    CreateEmbed(Context, $"Removed the role **{role}** from {user.Mention}!"));
                return;
            }

            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"**{role}** doesn't exist on this server!"));
        }
    }
}