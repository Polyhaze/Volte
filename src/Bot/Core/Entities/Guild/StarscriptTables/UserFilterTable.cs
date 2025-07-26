namespace Volte.Entities;

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
}

public class UserFilterEntry
{
    public uint Id { get; set; }
    
    public StarscriptSrc Starscript { get; set; } = new();
    
    public ActionType Action { get; set; }
    
    public string Reason { get; set; }
    
    /// <summary>
    ///     Performs the action specified by <see cref="Action"/> with the given <see cref="Reason"/>.
    /// </summary>
    /// <param name="guild">The guild the user is in.</param>
    /// <param name="user">The user to moderate.</param>
    /// <returns>true if an action was carried out successfully; false if there was an <see cref="HttpException"/>.</returns>
    public async Task<bool> ExecuteAsync(SocketGuild guild, IGuildUser user)
    {
        switch (Action)
        {
            case ActionType.Kick:
                try
                {
                    await user.KickAsync(Reason,
                        DiscordHelper.RequestOptions(x => x.AuditLogReason = $"Triggered user filter {Id}"));
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
                    return true;
                }
                catch (HttpException)
                {
                    return false;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(Action));
        }
    }
}

public enum ActionType 
{
    Kick,
    Ban,
    SoftBan
}