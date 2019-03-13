using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("Usage: |prefix|remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task RemRoleAsync(SocketGuildUser user, [Remainder] SocketRole role)
        {
            await user.RemoveRoleAsync(role);
            await Context.CreateEmbed($"Removed the role **{role.Name}** from {user.Mention}!")
                .SendToAsync(Context.Channel);
        }
    }
}