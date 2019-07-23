using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        [RequireGuildAdmin, RequireBotGuildPermission(GuildPermission.ManageRoles)]
        public async Task<VolteCommandResult> AddRoleAsync(SocketGuildUser user, [Remainder] SocketRole role)
        {
            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
                return BadRequest("Role position is too high for me to be able to grant it to anyone.");

            await user.AddRoleAsync(role);
            return Ok($"Added the role **{role}** to {user.Mention}!");
        }
    }
}