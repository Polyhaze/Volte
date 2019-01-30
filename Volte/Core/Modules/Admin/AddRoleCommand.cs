using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("AddRole"), Alias("Ar")]
        [Summary("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        public async Task AddRole(SocketGuildUser user, [Remainder] string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var targetRole = Context.Guild.Roles.FirstOrDefault(r => string.Equals(r.Name, role, StringComparison.CurrentCultureIgnoreCase));
            if (targetRole != null) {
                await user.AddRoleAsync(targetRole);
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"Added the role **{role}** to {user.Mention}!"));
                return;
            }

            await Reply(Context.Channel,
                CreateEmbed(Context, $"**{role}** doesn't exist on this server!"));
        }
    }
}