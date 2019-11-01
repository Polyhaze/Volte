using Discord;
using System.Threading.Tasks;
using Qmmands;
using SixLabors.ImageSharp.PixelFormats;
using Volte.Commands.Results;
using Volte.Helpers;
using System.Text;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("ShowColor", "Sc")]
        [Description("Shows an image purely made up of the specified color.")]
        [Remarks("showcolor")]
        public Task<ActionResult> ShowColorAsync([Remainder]Color color)
            => Ok(async () =>
            {
                await using var stream = ImageHelper.CreateColorImage(new Rgba32(color.R, color.G, color.B));
                await Context.Channel.SendFileAsync(stream, "role.png", null, embed: new EmbedBuilder()
                    .WithColor(color)
                    .WithTitle($"Color {color}")
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Hex:** {color.ToString().ToUpper()}")
                        .AppendLine($"**RGB:** {color.R}, {color.G}, {color.B}")
                        .ToString())
                    .WithImageUrl("attachment://role.png")
                    .WithCurrentTimestamp()
                    .Build());
            });
    }
}
