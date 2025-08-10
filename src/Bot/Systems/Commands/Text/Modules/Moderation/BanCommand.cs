namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Ban")]
    [Description("Bans a member.")]
    public async Task<ActionResult> BanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")]
        SocketGuildUser member,
        [Remainder, Description("The reason for the ban.")]
        string reason = null)
    {
        var e = Context.CreateEmbedBuilder(reason is not null
            ? $"You've been banned from **{Context.Guild.Name}** for **{reason}**."
            : $"You've been banned from **{Context.Guild.Name}**."
        );
        
        if (!await member.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

        try
        {
            await member.BanAsync(7, GetReason("Banned", Context.User, reason));
            return Ok($"Successfully banned **{member}** from this guild.", 
                Context.ModAction
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(member)
                    .WithReason(reason)
                );
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}