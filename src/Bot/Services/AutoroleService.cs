using Volte.Systems.Database;

namespace Volte.Services;

public sealed class AutoroleService : VolteService
{
    private readonly DatabaseService _db;

    public AutoroleService(DatabaseService databaseService)
    {
        _db = databaseService;
    }

    public async Task ApplyRoleAsync(UserJoinedEventArgs args)
    {
        if (!Config.EnabledFeatures.Autorole)
            return;
        
        var data = _db.GetData(args.Guild);
        var targetRole = args.Guild.GetRole(data.Settings.Autorole);
        if (targetRole is null)
        {
            Debug(LogSource.Volte,
                $"Guild {args.Guild.Name}'s Autorole is set to an ID of a role that no longer exists; or is not set at all.");
            return;
        }

        await args.User.AddRoleAsync(targetRole, new RequestOptions { AuditLogReason = "Volte Autorole" });
        Debug(LogSource.Volte,
            $"Applied role {targetRole.Name} to user {args.User} in guild {args.Guild.Name}.");
    }
}