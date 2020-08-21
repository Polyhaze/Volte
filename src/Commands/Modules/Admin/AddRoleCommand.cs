using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("addrole {Member} {Role}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public async Task<ActionResult> AddRoleAsync(DiscordMember user, [Remainder] DiscordRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
                return BadRequest("Role position is too high for me to be able to grant it to anyone.");

            await user.GrantRoleAsync(role);
            return Ok($"Added the role **{role}** to {user.Mention}!");
        }
    }
}