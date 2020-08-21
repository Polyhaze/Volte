using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("remrole {Member} {Role}")]
        public async Task<ActionResult> RemRoleAsync(DiscordMember user, [Remainder] DiscordRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
            {
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");
            }

            await user.RevokeRoleAsync(role);
            return Ok($"Removed the role **{role.Name}** from {user.Mention}!");
        }
    }
}