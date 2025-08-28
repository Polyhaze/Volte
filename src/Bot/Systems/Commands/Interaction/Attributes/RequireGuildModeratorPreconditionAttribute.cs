using Discord.Interactions;
using Volte.Systems.Database;

namespace Volte.Systems.Commands.Interaction;

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

        var u = 
            context.Guild.Cast<SocketGuild>()?.GetUser(context.User.Id) 
            ?? await context.Guild.GetUserAsync(context.User.Id);

        return u.RoleIds.Contains(data.Settings.Moderation.ModRole) 
               || u.RoleIds.Contains(data.Settings.Moderation.AdminRole) 
               || u.Guild.OwnerId == u.Id
               || u.IsBotOwner()
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("This command requires you to be a moderator.");
    }
}