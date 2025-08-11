namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("ReBan")]
    [Description("Unbans a user by their ID, then bans them again with a new reason.")]
    public async Task<ActionResult> ReBanAsync(
        [CheckHierarchy, EnsureNotSelf, Description("The ID of the user to re-ban.")]
        RestUser user,
        [Remainder, Description("The new reason for the ban.")]
        string reason)
    {
        var isBanned = await Context.Guild.GetBanAsync(user) != null;

        var message = isBanned
            ? $"Successfully re-banned **{user}** from this guild."
            : $"Successfully banned **{user}** from this guild.";

        if (isBanned)
        {
            await Context.Guild.RemoveBanAsync(user,
                new RequestOptions { AuditLogReason = "First half of re-ban action; see subsequent ban." });
        }

        await Context.Guild.AddBanAsync(user, 7, reason);
        return Ok(message, Context.ModAction
            .WithActionType(ModActionType.Ban)
            .WithTarget(user)
            .WithReason(reason)
        );
    }
}