using LiteDB;
using Volte.Systems.Database.Entities;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.Starboard;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Volte.Systems.Database;

public sealed class DatabaseService : VolteService, IDisposable
{
    public static readonly LiteDatabase Database =
        new($"filename={FilePath.Data / "Volte.db"};upgrade=true;connection=direct");

    private readonly DiscordSocketClient _client;

    private readonly ILiteCollection<GuildData> _guildData;
    private readonly ILiteCollection<GuildDataV2> _guildDataV2;
    private readonly ILiteCollection<Reminder.Reminder> _reminderData;
    private readonly ILiteCollection<StarboardDbEntry> _starboardData;

    public DatabaseService(DiscordSocketClient discordShardedClient)
    {
        _client = discordShardedClient;
        _guildData = Database.GetCollection<GuildData>("guilds");
        _guildDataV2 = Database.GetCollection<GuildDataV2>("guildData");
        _reminderData = Database.GetCollection<Reminder.Reminder>("reminders");
        _starboardData = Database.GetCollection<StarboardDbEntry>("starboard");
        _starboardData.EnsureIndex("composite_id",
            $"$.{nameof(StarboardDbEntry.GuildId)} + '_' + $.{nameof(StarboardDbEntry.Key)}");
    }

    public int Migrate()
    {
        var v1 = _guildData.ValueLock(() => _guildData.FindAll().ToHashSet());
        
        return _guildDataV2.ValueLock(() =>
        {
            _guildDataV2.DeleteAll();
            return _guildDataV2.InsertBulk(v1.Select(GuildDataV2.MigrateFromV1));
        });
    }

    public GuildData GetData(IGuild guild) => GetData(guild.Id);

    public ValueTask<GuildData> GetDataAsync(ulong id) => new(GetData(id));

    public GuildData GetData(ulong id)
    {
        return _guildData.ValueLock(() =>
        {
            var conf = _guildData.FindOne(g => g.Id == id);
            if (conf != null) return conf;
            var newConf = GuildData.CreateFrom(_client.GetGuild(id));
            _guildData.Insert(newConf);
            return newConf;
        });
    }

    public void Modify(ulong guildId, DataEditor modifier)
    {
        _guildData.LockedRef(coll =>
        {
            var data = GetData(guildId);
            modifier(data);
            Save(data);
        });
    }

    public void Save(GuildData newConfig)
    {
        _guildData.LockedRef(coll =>
        {
            coll.EnsureIndex(s => s.Id, true);
            coll.Update(newConfig);
        });
    }
    
    public HashSet<Reminder.Reminder> this[IUser user] => GetReminders(user);
    
    public HashSet<Reminder.Reminder> GetReminders(IUser user, IGuild guild = null) =>
        GetReminders(user.Id, guild?.Id ?? 0).ToHashSet();

    public HashSet<Reminder.Reminder> GetReminders(ulong creator, ulong guild = 0)
        => GetAllReminders().Where(r => r.CreatorId == creator && (guild is 0 || r.GuildId == guild)).ToHashSet();

    public bool TryDeleteReminder(Reminder.Reminder reminder) =>
        _reminderData.ValueLock(() => _reminderData.Delete(reminder.Id));

    public HashSet<Reminder.Reminder> GetAllReminders() => _reminderData.ValueLock(() => _reminderData.FindAll().ToHashSet());

    public void CreateReminder(Reminder.Reminder reminder) => _reminderData.ValueLock(() => _reminderData.Insert(reminder));

    public async Task WarnAsync(IUser issuer, IGuildUser member, string reason)
    {
        var data = await GetDataAsync(member.GuildId);

        data.Extras.Warns.Add(new Warn
        {
            User = member.Id,
            Reason = reason,
            Issuer = issuer.Id,
            Date = DateTimeOffset.Now
        });
        
        Save(data);

        var e = new EmbedBuilder().WithSuccessColor().WithAuthor(issuer)
            .WithDescription($"You've been warned in {Format.Bold(member.Guild.Name)} for {Format.Code(reason)}.")
            .Apply(data);

        if (!await member.TrySendMessageAsync(embed: e.Build()))
            Warn(LogSource.Service, $"encountered a 403 when trying to message {member}!");
    }

    private StarboardDbEntry GetStargazersInternal(ulong guildId, ulong messageId)
        => _reminderData.ValueLock(() => _starboardData.FindOne(g => g.GuildId == guildId && g.Key == messageId));

    public StarboardEntry GetStargazers(ulong guildId, ulong messageId)
        => GetStargazersInternal(guildId, messageId)?.Value;


    public bool TryGetStargazers(ulong guildId, ulong messageId, [NotNullWhen(true)] out StarboardEntry entry)
    {
        entry = GetStargazersInternal(guildId, messageId)?.Value;
        return entry != null;
    }

    public void UpdateStargazers(StarboardEntry entry)
    {
        _starboardData.LockedRef(coll =>
        {
            coll.Upsert($"{entry.GuildId}_{entry.StarboardMessageId}", new StarboardDbEntry
            {
                GuildId = entry.GuildId,
                Key = entry.StarboardMessageId,
                Value = entry
            });

            coll.Upsert($"{entry.GuildId}_{entry.StarredMessageId}", new StarboardDbEntry
            {
                GuildId = entry.GuildId,
                Key = entry.StarredMessageId,
                Value = entry
            });
        });
    }

    public void RemoveStargazers(StarboardEntry entry)
    {
        _starboardData.LockedRef(coll =>
        {
            coll.Delete($"{entry.GuildId}_{entry.StarboardMessageId}");
            coll.Delete($"{entry.GuildId}_{entry.StarredMessageId}");
        });
    }

    public void Dispose()
        => Database.Dispose();
}