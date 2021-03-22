using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        public async Task<ActionResult> RemRoleAsync([Description("The user to remove the role from.")] SocketGuildUser user, [Remainder, Description("The role to remove.")] SocketRole role)
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