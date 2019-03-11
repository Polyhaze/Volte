using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task AddRoleAsync(SocketGuildUser user, [Remainder] SocketRole role)
        {
            await user.AddRoleAsync(role);
            await Context.CreateEmbed($"Added the role **{role}** to {user.Mention}!")
                .SendToAsync(Context.Channel);
        }
    }
}