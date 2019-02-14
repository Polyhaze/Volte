using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("Usage: |prefix|remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task RemRole(SocketGuildUser user, [Remainder] string role) {
            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
            if (targetRole != null) {
                await user.RemoveRoleAsync(targetRole);
                await Context.CreateEmbed($"Removed the role **{role}** from {user.Mention}!").SendTo(Context.Channel);
                return;
            }
            await Context.CreateEmbed($"**{role}** doesn't exist on this server!").SendTo(Context.Channel);
        }
    }
}