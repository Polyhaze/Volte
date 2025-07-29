namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("IdBan")]
    [Description("Bans a user based on their ID.")]
    public async Task<ActionResult> IdBanAsync([Description("The ID of the user to ban.")]
        ulong user,
        [Remainder, Description("The reason for the ban.")]
        string reason = "Banned by a Moderator.")
    {
        await Context.Guild.AddBanAsync(user, 0, reason);
        return Ok($"Successfully banned **{await Context.Client.Rest.GetUserAsync(user)}** from this guild.",
            async _ =>
                await ModerationService.OnModActionCompleteAsync(ModActionEventArgs
                    .InContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(user)
                    .WithReason(reason))
        );
    }
}