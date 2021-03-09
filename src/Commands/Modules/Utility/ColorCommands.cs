using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current guild.")]
        [Remarks("color {Role}")]
        public Task<ActionResult> RoleColorAsync([Remainder] SocketRole role)
        {
            if (!role.HasColor()) return BadRequest("Role does not have a color.");

            return Ok(async () =>
            {
                await using var stream = new Rgba32(role.Color.R, role.Color.G, role.Color.B).CreateColorImage();
                await stream.SendFileToAsync(Context.Channel, "role.png", "", false, new EmbedBuilder()
                    .WithColor(role.Color)
                    .WithTitle("Role Color")
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Hex:** {role.Color.ToString().ToUpper()}")
                        .AppendLine($"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}")
                        .ToString())
                    .WithImageUrl("attachment://role.png")
                    .WithCurrentTimestamp()
                    .Build(),
                    reference: new MessageReference(Context.Message.Id));
            });

        }
        
        [Command("ShowColor", "Sc")]
        [Description("Shows an image purely made up of the specified color.")]
        [Remarks("showcolor {Color}")]
        public Task<ActionResult> ShowColorAsync([Remainder]Color color)
            => Ok(async () =>
            {
                await using var stream = new Rgba32(color.R, color.G, color.B).CreateColorImage();
                await stream.SendFileToAsync(Context.Channel, "role.png", "", false, new EmbedBuilder()
                        .WithColor(color)
                        .WithTitle($"Color {color}")
                        .WithDescription(new StringBuilder()
                            .AppendLine($"**Hex:** {color.ToString().ToUpper()}")
                            .AppendLine($"**RGB:** {color.R}, {color.G}, {color.B}")
                            .ToString())
                        .WithImageUrl("attachment://role.png")
                        .WithCurrentTimestamp()
                        .Build(),
                    reference: new MessageReference(Context.Message.Id));
            });
    }
}