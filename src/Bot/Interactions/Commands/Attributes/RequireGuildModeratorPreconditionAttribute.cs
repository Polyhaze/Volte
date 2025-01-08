using Discord.Interactions;

namespace Volte.Interactions.Commands;

public class RequireGuildModeratorPreconditionAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
    )
    {
        if (context.Guild is null)
            return PreconditionResult.FromError("This command can only be executed in a guild.");

        var db = services.Get<DatabaseService>();
        var data = await db.GetDataAsync(context.Guild.Id);

        var u = await context.Guild.GetUserAsync(context.User.Id);

        return u.RoleIds.Contains(data.Configuration.Moderation.ModRole) 
               || u.RoleIds.Contains(data.Configuration.Moderation.AdminRole) 
               || u.Guild.OwnerId == u.Id
               || u.IsBotOwner()
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("This command requires you to be a moderator.");
    }
}