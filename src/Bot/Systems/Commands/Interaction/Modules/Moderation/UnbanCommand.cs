using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("unban", "Unban a user by their ID.")]
    public async Task<RuntimeResult> UnbanAsync(
        [Summary("user_id", "The ID of the user to unban.")]
        string userId,
        [Summary("reason", "The reason for the unban.")]
        string reason)
    {
        await DeferAsync();
        
        if (!ulong.TryParse(userId, out var userIdUL))
            return BadRequest("user_id is not a valid Discord user ID.");

        var user = await Context.Client.Rest.GetUserAsync(userIdUL);
        if (user is null) 
            return BadRequest("User not found");

        var ban = await Context.Guild.GetBanAsync(user);

        if (ban is null)
            return BadRequest($"**{user}** is not banned.");

        await Context.Guild.RemoveBanAsync(userIdUL, 
            new RequestOptions
            {
                AuditLogReason = GetReason(Context.User, reason)
            }
        );
        
        return Ok($"Successfully unbanned **{user}** from this guild.", () =>
            ModService.OnModActionCompleteAsync(ModActionEventArgs
                .FromModule(this)
                .WithActionType(ModActionType.Unban)
                .WithTarget(user)
                .WithReason(reason))
        );
    }
}