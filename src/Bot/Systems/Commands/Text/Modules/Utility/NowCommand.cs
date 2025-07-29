namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Now")]
    [Description("Shows the current date and time.")]
    public Task<ActionResult> NowAsync()
        => Ok(new EmbedBuilder().WithTitle(Context.Message.CreatedAt.ToDiscordTimestamp(TimestampType.LongDateTime)));
}