using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role. Accepts a Hex or RGB value.")]
        [Remarks("rolecolor {Role} {Color}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public async Task<ActionResult> RoleColorAsync(DiscordRole role, [Remainder] DiscordColor color)
        {
            await role.ModifyAsync(x => x.Color = color);
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }
    }
}