namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Snowflake", "Id")]
    [Description("Shows when the object with the given Snowflake ID was created. Adjusts to your timezone.")]
    public Task<ActionResult> SnowflakeAsync([Description("The Discord snowflake you want to see.")] ulong id) =>
        Ok(Context.CreateEmbedBuilder().WithTitle(SnowflakeUtils.FromSnowflake(id).ToDiscordTimestamp(TimestampType.LongDateTime)));
}