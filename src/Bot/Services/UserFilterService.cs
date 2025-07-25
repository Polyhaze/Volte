using Starscript.Internal;

namespace Volte.Services;

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
        var idsToClear = await HandleMultipleScripts(args.Guild, args.User, data.Extras.StarscriptTables.UserFilter).ToArrayAsync();

        if (idsToClear.Length > 0)
        {
            data.Extras.StarscriptTables.UserFilter.Entries.RemoveAll(it => idsToClear.Contains(it.Id));
            _db.Save(data);
        }
    }
    
    private async IAsyncEnumerable<uint> HandleMultipleScripts(SocketGuild guild, SocketGuildUser user, UserFilterTable table)
    {
        foreach (var entry in table.Entries)
        {
            if (await HandleSingleScript(guild, user, entry) is { } id)
                yield return id;
        }
    }
    
    private async Task<uint?> HandleSingleScript(SocketGuild guild, SocketGuildUser user, UserFilterEntry entry)
    {
        try
        {
            var script = entry.Starscript.Compile();

            if (VolteStarscript.Hypervisor.Run(script, StarscriptHelper.Wrap(user)).GetBooleanValue())
            {
                if (await entry.ExecuteAsync(guild, user))
                {
                    Debug(LogSource.Service, $"{user} matched filter {entry.Id} in guild {user.Guild.Id}");
                }
            }

            return null;
        }
        catch (ParseException)
        {
            // Remove entries that have parsing errors
            return entry.Id;
        }
        catch (FormatException)
        {
            // Remove entries that do not return true/false
            return entry.Id;
        }
    }
}