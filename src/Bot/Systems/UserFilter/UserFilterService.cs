using Volte.Systems.Database;

namespace Volte.Systems.UserFilter;

public class UserFilterService : VolteService
{
    private readonly DatabaseService _db;

    public UserFilterService(DatabaseService databaseService)
    {
        _db = databaseService;
    }

    public async Task OnUserJoined(UserJoinedEventArgs args)
    {
        var data = await _db.GetDataAsync(args.Guild.Id);
        var idsToClear = await data.Extras.StarscriptTables.UserFilter
            .HandleAsync(args.Guild, args.User)
            .ToArrayAsync();

        if (idsToClear.Length > 0)
        {
            data.Extras.StarscriptTables.UserFilter.Entries.RemoveAll(it => idsToClear.Contains(it.Id));
            _db.Save(data);
        }
    }
}