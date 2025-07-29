using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("banid", "Ban a user by their ID.")]
    public async Task<RuntimeResult> IdBanAsync(
        [Summary("user_id", "The ID of the user to ban.")]
        string userId,
        [Summary("reason", "The reason for the ban.")]
        string reason)
    {
        await DeferAsync();
        
        if (!ulong.TryParse(userId, out var userIdUL))
            return BadRequest("user_id is not a valid Discord user ID.");

        var user = await Context.Client.Rest.GetUserAsync(userIdUL);
        if (user is null) 
            return BadRequest("User not found");

        var targetMember = await ((IGuild)Context.Guild).GetUserAsync(userIdUL);

        if (targetMember != null)
        {
            if (targetMember.Hierarchy >= ((IGuildUser)Context.User).Hierarchy ||
                targetMember.Hierarchy >= (await ((IGuild)Context.Guild).GetCurrentUserAsync()).Hierarchy)
                return BadRequest("You cannot target anyone who is higher or equal in the hierarchy to you or the bot.");
        }

        await Context.Guild.AddBanAsync(userIdUL, 0, reason);
        return Ok($"Successfully banned **{user}** from this guild.", () =>
                ModService.OnModActionCompleteAsync(ModActionEventArgs
                    .FromModule(this)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(userIdUL)
                    .WithReason(reason))
        );
    }
}