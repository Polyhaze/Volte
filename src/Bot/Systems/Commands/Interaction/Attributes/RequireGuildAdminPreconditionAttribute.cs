using Discord.Interactions;
using Volte.Systems.Database;

namespace Volte.Systems.Commands.Interaction;

public class RequireGuildAdminPreconditionAttribute : PreconditionAttribute
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
        var data = db.GetData(context.Guild.Id);
        
        var u = await context.Guild.GetUserAsync(context.User.Id);

        return u.RoleIds.Contains(data.Settings.Moderation.AdminRole) 
               || context.Guild.OwnerId == u.Id
               || u.IsBotOwner()
            ? PreconditionResult.FromSuccess() 
            : PreconditionResult.FromError("This command requires you to be an administrator.");
    }
}