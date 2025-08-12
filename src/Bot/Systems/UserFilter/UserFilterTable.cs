using Starscript;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.Starscript;

namespace Volte.Systems.UserFilter;

public class UserFilterTable
{
    public uint LastEntryId { get; set; }
    public List<UserFilterEntry> Entries { get; set; } = [];

    public void Add(string condition, ActionType actionType, string reason) =>
        Entries.Add(new UserFilterEntry
        {
            Id = LastEntryId++,
            Action = actionType,
            Reason = reason,
            Starscript = new StarscriptSrc
            {
                CodeInput = condition,
                Type = StarscriptType.SingleExpression
            }
        });
    
    public async IAsyncEnumerable<uint> HandleAsync(IGuild guild, IGuildUser user)
    {
        Debug(LogSource.Service, $"Running user filters on {user} in guild {user.Guild.Id}");
        
        foreach (var entry in Entries)
        {
            if (!await entry.HandleAsync(guild, user))
                yield return entry.Id;
        }
    }
}

public class UserFilterEntry
{
    public uint Id { get; set; }
    
    public StarscriptSrc Starscript { get; set; } = new();
    
    public ActionType Action { get; set; }
    
    public string Reason { get; set; }

    /// <summary>
    ///     Checks if this entry matches the proviced <paramref name="user"/> in <paramref name="guild"/>, and if they do, calls <see cref="ExecuteAsync"/> on them.
    /// </summary>
    /// <param name="guild">The guild to perform moderation actions from.</param>
    /// <param name="user">The user to filter.</param>
    /// <returns>true if the underlying <see cref="StarscriptSrc"/> compilation and execution succeeds, AND returns 'true' or 'false'; false otherwise.</returns>
    public async Task<bool> HandleAsync(IGuild guild, IGuildUser user)
    {
        StringSegment returnVal = null;

        try
        {
            using (var script = Starscript.Compile(guild))
            {
                returnVal = VolteStarscript.Hypervisor.Run(script, StarscriptHelper.Wrap(user));
            }

            if (returnVal.GetBooleanValue())
            {
                Debug(LogSource.Service, $"{user} matched filter {Id} in guild {user.Guild.Id}");

                _ = await ExecuteAsync(guild, user);
            }
            else
            {
                Debug(LogSource.Service, $"{user} did not match filter {Id} in guild {user.Guild.Id}");
            }

            return true;
        }
        catch (ParseException e)
        {
            // Remove entries that have parsing errors
            Debug(LogSource.Service, $"Filter {Id} in guild {user.Guild.Id} failed parsing:");
            Debug(LogSource.Service, e.Error.ToString());
            return false;
        }
        catch (StarscriptException e)
        {
            // Remove entries that have execution errors
            Debug(LogSource.Service, $"Filter {Id} in guild {user.Guild.Id} failed execution:");
            Debug(LogSource.Service, e.Message);
            return false;
        }
        catch (FormatException e)
        {
            // Remove entries that do not return true/false
            Debug(LogSource.Service, 
                $"Filter {Id} in guild {user.Guild.Id}'s result was not true/false:" +
                $"{(returnVal != null ? $" '{returnVal}'" : string.Empty)}");
            if (!e.Message.IsNullOrEmpty()) 
                Debug(LogSource.Service, e.Message);
            return false;
        }
    }
    
    /// <summary>
    ///     Performs the action specified by <see cref="Action"/> with the given <see cref="Reason"/>.
    /// </summary>
    /// <param name="guild">The guild the user is in.</param>
    /// <param name="user">The user to moderate.</param>
    /// <returns>true if an action was carried out successfully; false if there was an <see cref="HttpException"/>.</returns>
    public async Task<bool> ExecuteAsync(IGuild guild, IGuildUser user)
    {
        switch (Action)
        {
            case ActionType.Warn:
                try
                {
                    await user.WarnAsync(VolteBot.Client.CurrentUser, VolteBot.Services, Reason);
                    Debug(LogSource.Service, $"Warned {user} in guild {user.Guild.Id} because they matched filter {Id}");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            case ActionType.Kick:
                try
                {
                    await user.KickAsync(Reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = $"Triggered user filter {Id}"));
                    Debug(LogSource.Service, $"Kicked {user} in guild {user.Guild.Id} because they matched filter {Id}");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            case ActionType.Ban:
                try
                {
                    await guild.AddBanAsync(user.Id, 7, Reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = $"Triggered user filter {Id}"));
                    Debug(LogSource.Service, $"Banned {user} in guild {user.Guild.Id} because they matched filter {Id}");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            case ActionType.SoftBan:
                try
                {
                    await guild.AddBanAsync(user.Id, 7, Reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = $"Triggered user filter {Id}"));
                    await guild.RemoveBanAsync(user.Id);
                    Debug(LogSource.Service, $"Softbanned {user} in guild {user.Guild.Id} because they matched filter {Id}");
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            default:
                Debug(LogSource.Service, $"Unknown action type {Enum.GetName(Action)} for filter {Id}");
                return false;
        }
    }
}

public enum ActionType 
{
    Warn,
    Kick,
    Ban,
    SoftBan
}