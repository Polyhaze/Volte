using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current server.")]
        [Remarks("Usage: |prefix|color {role}")]
        public Task<ActionResult> RoleColorAsync([Remainder] SocketRole role)
        {
            if (role.Color.RawValue is 0) return BadRequest("Role does not have a color.");
            return Ok(Context.CreateEmbedBuilder().WithDescription(new StringBuilder()
                        .AppendLine($"**Hex:** {role.Color.ToString().ToUpper()}")
                        .AppendLine($"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}")
                        .ToString())
                    .WithColor(role.Color));

        }
    }
}