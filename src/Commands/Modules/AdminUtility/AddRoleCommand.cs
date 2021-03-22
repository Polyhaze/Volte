using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles)]
        public async Task<ActionResult> AddRoleAsync([Description("The user to add the role to.")] SocketGuildUser user, [Remainder, Description("The role to give the user.")] SocketRole role)
        {
            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
                return BadRequest("Role position is too high for me to be able to grant it to anyone.");

            await user.AddRoleAsync(role);
            return Ok($"Added the role **{role}** to {user.Mention}!");
        }
    }
}