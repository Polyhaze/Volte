using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task AddRoleAsync(SocketGuildUser user, [Remainder] string role)
        {
            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
            if (targetRole != null)
            {
                await user.AddRoleAsync(targetRole);
                await Context.CreateEmbed($"Added the role **{role}** to {user.Mention}!")
                    .SendToAsync(Context.Channel);
                return;
            }

            await Context.CreateEmbed($"**{role}** doesn't exist on this server!")
                .SendToAsync(Context.Channel);
        }
    }
}