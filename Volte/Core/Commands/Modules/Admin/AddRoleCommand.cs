using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task AddRole(SocketGuildUser user, [Remainder] string role) {
            var targetRole = Context.Guild.Roles.FirstOrDefault(r =>
                string.Equals(r.Name, role, StringComparison.CurrentCultureIgnoreCase));
            if (targetRole != null) {
                await user.AddRoleAsync(targetRole);
                await Context.CreateEmbed($"Added the role **{role}** to {user.Mention}!")
                    .SendTo(Context.Channel);
                return;
            }

            await Context.CreateEmbed($"**{role}** doesn't exist on this server!")
                .SendTo(Context.Channel);
        }
    }
}