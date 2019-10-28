using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task<ActionResult> RemRoleAsync(SocketGuildUser user, [Remainder] SocketRole role)
        {
            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");
            }

            await user.RemoveRoleAsync(role);
            return Ok($"Removed the role **{role.Name}** from {user.Mention}!");
        }
    }
}