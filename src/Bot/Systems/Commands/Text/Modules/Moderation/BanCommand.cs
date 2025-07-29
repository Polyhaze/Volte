namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Ban")]
    [Description("Bans a member.")]
    public async Task<ActionResult> BanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")]
        SocketGuildUser member,
        [Remainder, Description("The reason for the ban.")]
        string reason = "Banned by a Moderator.")
    {
        var e = Context
            .CreateEmbedBuilder(
                $"You've been banned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.");

        if (!await member.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

        try
        {
            await member.BanAsync(7, reason);
            return Ok($"Successfully banned **{member}** from this guild.", _ =>
                ModerationService.OnModActionCompleteAsync(ModActionEventArgs.InContext(Context)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(member)
                    .WithReason(reason))
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }

    [Command("UnixBan", "UBan")]
    [Description("Bans the user with custom modifications provided via Unix arguments.")]
    [ShowUnixArgumentsInHelp(VolteUnixCommand.UnixBan)]
    public async Task<ActionResult> UnixBanAsync([CheckHierarchy, EnsureNotSelf, Description("The member to ban.")]
        SocketGuildUser member,
        [Remainder, Description("The modifications to the ban action you'd like to make.")]
        Dictionary<string, string> modifications)
    {
        var daysToDelete = (modifications.TryGetValue("days", out var result) ||
                            modifications.TryGetValue("deleteDays", out result)) &&
                           result.TryParse<int>(out var intResult)
            ? intResult.CoerceAtMost(7).CoerceAtLeast(0)
            : 0;

        var reason = modifications.TryGetValue("reason", out result) ? result : "Banned by a Moderator.";

        var e = Context
            .CreateEmbedBuilder(
                $"You've been banned from {Format.Bold(Context.Guild.Name)} for {Format.Bold(reason)}.");

        if (!Context.GuildData.Configuration.Moderation.ShowResponsibleModerator || modifications.TryGetValue("shadow", out _))
        {
            e = e.WithAuthor(author: null).WithSuccessColor();
        }
            
        if (!await member.TrySendMessageAsync(embed: e.Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

        try
        {
            await member.BanAsync(daysToDelete, reason);
            if (modifications.TryGetValue("soft", out _) || modifications.TryGetValue("softly", out _))
                await Context.Guild.RemoveBanAsync(member.Id);
                
            return Ok($"Successfully banned **{member}** from this guild.", _ =>
                ModerationService.OnModActionCompleteAsync(ModActionEventArgs
                    .InContext(Context)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(member)
                    .WithReason(reason))
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}