using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned member.")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles)]
        public async Task<ActionResult> RemRoleAsync([Description("The member to remove the role from.")]
            SocketGuildUser member, [Remainder, Description("The role to remove from the member.")]
            SocketRole role)
        {
            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");

            await member.RemoveRoleAsync(role);
            return Ok($"Removed the role **{role.Name}** from {member.Mention}!");
        }
    }
}