using Volte.Systems.Database;

namespace Volte.Systems.UserFilter;

public class UserFilterService : VolteService
{
    private readonly DatabaseService _db;

    public UserFilterService(DatabaseService databaseService)
    {
        _db = databaseService;
    }

    public Task OnUserJoined(UserJoinedEventArgs args)
    {
        ExecuteBackgroundAsync(async () =>
        {
            var data = _db.GetData(args.Guild.Id);
            var idsToClear = await data.StarscriptTables.UserFilter
                .HandleAsync(args.Guild, args.User)
                .ToArrayAsync();

            if (idsToClear.Length > 0)
            {
                data.StarscriptTables.UserFilter.Entries.RemoveAll(it => idsToClear.Contains(it.Id));
                _db.Save(data);
            }
        });

        return Task.CompletedTask;
    }
}