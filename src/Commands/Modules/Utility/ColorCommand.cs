using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Commands.TypeParsers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current guild; or just a color.")]
        [Remarks("color {Color/Role}")]
        public async Task<ActionResult> RoleColorAsync([Remainder] string colorOrRole)
        {
            var roleTypeParse = await new RoleParser().ParseAsync(null, colorOrRole, Context);
            if (roleTypeParse.IsSuccessful)
            {
                var role = roleTypeParse.Value;
                if (!role.HasColor()) return BadRequest("Role does not have a color.");

                return Ok(async () =>
                {
                    await using var stream = new Rgba32(role.Color.R, role.Color.G, role.Color.B).CreateColorImage();
                    await stream.SendFileToAsync(Context.Channel, "role.png", "", false, new EmbedBuilder()
                            .WithColor(role.Color)
                            .WithTitle($"{role.Name}'s Color")
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

            var colorTypeParse = await new ColorParser().ParseAsync(null, colorOrRole, Context);
            // ReSharper disable once InvertIf
            if (colorTypeParse.IsSuccessful)
            {
                var color = colorTypeParse.Value;
                return Ok(async () =>
                {
                    await using var stream = new Rgba32(color.R, color.G, color.B).CreateColorImage();
                    await stream.SendFileToAsync(Context.Channel, "role.png", "", false, new EmbedBuilder()
                            .WithColor(color)
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

            return BadRequest("You didn't give a valid role or color.");
        }
    }
}