using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role. Accepts a Hex or RGB value.")]
        public async Task<ActionResult> RoleColorAsync([Description("The role to modify.")] SocketRole role,
            [Remainder, Description("The color to change the role to. Accepts #hex and RGB.")]
            Color color)
        {
            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
                return BadRequest("Role position is too high for me to be able to do anything with it.");

            await role.ModifyAsync(x => x.Color = color);
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }
    }
}