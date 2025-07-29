namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class BotOwnerModule
{
    [Command("ForceLeave")]
    [Description("Forcefully leaves the guild with the given name.")]
    public async Task<ActionResult> ForceLeaveAsync([Remainder, Description("The guild to leave.")] SocketGuild guild)
    {
        await guild.LeaveAsync();
        return Ok($"Successfully left **{guild.Name}**.");
    }
}