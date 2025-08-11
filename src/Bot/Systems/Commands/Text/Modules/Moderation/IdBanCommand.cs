namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("IdBan")]
    [Description("Bans a user based on their ID.")]
    public async Task<ActionResult> IdBanAsync(
        [CheckHierarchy, EnsureNotSelf, Description("The ID of the user to ban.")]
        RestUser user,
        [Remainder, Description("The reason for the ban.")]
        string reason = null)
    {
        await Context.Guild.AddBanAsync(user, 0, GetReason("Banned", Context.User, reason));
        return Ok($"Successfully banned **{user}** from this guild.", Context.ModAction
            .WithActionType(ModActionType.IdBan)
            .WithTarget(user)
            .WithReason(reason)
        );
    }
}