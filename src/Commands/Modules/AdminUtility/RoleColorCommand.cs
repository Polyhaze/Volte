using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role.")]
        [Remarks("Usage: |prefix|rolecolor {role} {r} {g} {b}")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles)]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> RoleColorAsync(SocketRole role, int r, int g, int b)
        {
            await role.ModifyAsync(x => x.Color = new Color(r, g, b));
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }
    }
}