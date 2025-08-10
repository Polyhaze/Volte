namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("Unban")]
    [Description("Unbans a user based on their ID.")]
    public async Task<ActionResult> UnbanAsync([Description("The ID of the user to unban.")]
        RestUser user,
        [Remainder, Description("The reason for the unban.")]
        string reason = null)
    {
        var ban = await Context.Guild.GetBanAsync(user);

        if (ban is null)
            return BadRequest($"**{user}** is not banned.");
        
        await Context.Guild.RemoveBanAsync(user, new RequestOptions { AuditLogReason = GetReason("Unbanned", Context.User, reason) });
        return Ok($"Successfully unbanned **{user}** from this guild.", Context.ModAction
            .WithActionType(ModActionType.Unban)
            .WithTarget(user)
            .WithReason(reason)
        );
    }
}