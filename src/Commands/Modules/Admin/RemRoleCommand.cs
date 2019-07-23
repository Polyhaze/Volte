using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Data.Models.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("Usage: |prefix|remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> RemRoleAsync(SocketGuildUser user, [Remainder] SocketRole role)
        {
            if (role.Position > (await Context.Guild.GetCurrentUserAsync()).Cast<SocketGuildUser>()?.Hierarchy)
            {
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");
            }

            await user.RemoveRoleAsync(role);
            return Ok($"Removed the role **{role.Name}** from {user.Mention}!");
        }
    }
}