using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current guild.")]
        [Remarks("color {Role}")]
        public async Task<ActionResult> RoleColorAsync([Remainder] DiscordRole role)
        {
            if (!role.HasColor()) return BadRequest("Role does not have a color.");
            
            await using var stream = new Rgba32(role.Color.R, role.Color.G, role.Color.B).CreateColorImage();
            await stream.SendFileToAsync(Context.Channel, "role.png", false, new DiscordEmbedBuilder()
                .WithColor(role.Color)
                .WithTitle("Role Color")
                .WithDescription(new StringBuilder()
                    .AppendLine($"**Hex:** {role.Color.ToString().ToUpper()}")
                    .AppendLine($"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}")
                    .ToString())
                .WithImageUrl("attachment://role.png")
                .WithCurrentTimestamp()
                .Build());
            return None();

        }
    }
}