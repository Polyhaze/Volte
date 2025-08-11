namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Kick")]
    [Description("Kicks the given user.")]
    public async Task<ActionResult> KickAsync([CheckHierarchy, EnsureNotSelf, Description("The member to kick.")]
        SocketGuildUser user,
        [Remainder, Description("The reason for the kick.")]
        string reason = null)
    {
        var e = Context.CreateEmbedBuilder(reason is not null
            ? $"You've been kicked from **{Context.Guild.Name}** for **{reason}**."
            : $"You've been kicked from **{Context.Guild.Name}**."
        );
        
        if (!await user.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {user}!");

        try
        {
            await user.KickAsync(GetReason("Kicked", Context.User, reason));
            return Ok($"Successfully kicked **{user}** from this guild.", Context.ModAction(ModActionType.Kick)
                .WithTarget(user)
                .WithReason(reason)
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred kicking that user. Do I have permission; or are they higher than me in the role list?");
        }
    }
}