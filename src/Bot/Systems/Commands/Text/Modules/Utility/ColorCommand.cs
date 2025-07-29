namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Color", "Colour", "C")]
    [Description("Shows the Hex and RGB representation for a given role in the current guild; or just a color.")]
    public Task<ActionResult> RoleColorAsync(
        [Remainder,
         Description("The color you want to see, in #hex or RGB, or a role whose color you want to be shown.")]
        string colorOrRole)
    {
        var roleTypeParse = RoleParser.Parse(colorOrRole, Context);
        if (roleTypeParse.IsSuccessful)
        {
            var role = roleTypeParse.Value;

            return !role.HasColor()
                ? BadRequest("Role does not have a color.")
                : Ok(async () =>
                {
                    await using var stream = role.Color.ToRgba32().CreateColorImage();
                    await stream.SendFileToAsync(Context.Channel, "role.png", string.Empty, false,
                        new EmbedBuilder()
                            .WithColor(role.Color)
                            .WithTitle($"{role.Name}'s Color")
                            .WithDescription(new StringBuilder()
                                .AppendLine($"**Hex:** {role.Color.ToString().ToUpper()}")
                                .AppendLine($"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}"))
                            .WithImageUrl("attachment://role.png")
                            .WithCurrentTimestamp()
                            .Build(),
                        reference: new MessageReference(Context.Message.Id));
                });
        }

        var colorTypeParse = ColorParser.Parse(colorOrRole);
        if (!colorTypeParse.IsSuccessful)
            return BadRequest("You didn't give a valid role or color.");
            
            
        var color = colorTypeParse.Value;
        return Ok(async () =>
        {
            await using var stream = color.ToRgba32().CreateColorImage();
            await stream.SendFileToAsync(Context.Channel, "role.png", embed: new EmbedBuilder()
                    .WithColor(color)
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Hex:** {color.ToString().ToUpper()}")
                        .AppendLine($"**RGB:** {color.R}, {color.G}, {color.B}")
                        .ToString())
                    .WithImageUrl("attachment://role.png")
                    .WithCurrentTimestamp()
                    .Build(),
                reference: new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild.Id));
        });
    }
}