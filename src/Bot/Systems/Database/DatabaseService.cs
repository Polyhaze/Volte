using LiteDB;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.Starboard;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Volte.Systems.Database;

public sealed class DatabaseService : VolteService, IDisposable
{
    public static readonly LiteDatabase Database =
        new($"filename={FilePath.Data / "Volte.db"};upgrade=true;connection=direct");

    private readonly DiscordSocketClient _client;

    private readonly ILiteCollection<Entities.GuildData> _guildData;
    private readonly ILiteCollection<GuildDataV2> _guildDataV2;
    private readonly ILiteCollection<Reminder.Reminder> _reminderData;
    private readonly ILiteCollection<StarboardDbEntry> _starboardData;

    public DatabaseService(DiscordSocketClient discordSocketClient)
    {
        BsonMapper.Global.RegisterType(
            serialize: it => it.Raw,
            deserialize: bVal => new Snowflake((ulong)bVal.AsInt64));
        
        _client = discordSocketClient;
        _guildData = Database.GetCollection<Entities.GuildData>("guilds");
        _guildDataV2 = Database.GetCollection<GuildDataV2>("guildData");
        _reminderData = Database.GetCollection<Reminder.Reminder>("reminders");
        _starboardData = Database.GetCollection<StarboardDbEntry>("starboard");
        _starboardData.EnsureIndex("composite_id",
            $"$.{nameof(StarboardDbEntry.GuildId)} + '_' + $.{nameof(StarboardDbEntry.Key)}");
    }

    public void Initialize()
    {
        if (Program.CommandLineArguments.ContainsKey("migrate"))
            _ = Migrate();
        else if (Program.CommandLineArguments.ContainsKey("hard-migrate"))
            _ = Migrate(destroyV1: true);
    }

    public int Migrate(bool destroyV1 = false)
    {
        var v1 = _guildData.ValueLock(() => _guildData.FindAll().ToHashSet());

        if (v1.None())
        {
            Info(LogSource.Service, "Guild data has already been migrated to V2.");
            return 0;
        }
        
        Info(LogSource.Service, $"Migrating {v1.Count} guild data entries...");

        if (destroyV1)
        {
            var cleared = _guildData.ValueLock(() => _guildData.DeleteAll());
            if (cleared > 0)
                Info(LogSource.Service, $"Cleared {"existing guild data V1 entry".ToQuantity(cleared)}.");

            Info(LogSource.Service, "Dropping 'guilds' database collection.");
            Database.DropCollection("guilds");
        }
        
        return _guildDataV2.ValueLock(() =>
        {
            var cleared = _guildDataV2.DeleteAll();
            
            if (cleared > 0)
                Info(LogSource.Service, $"Cleared {"existing guild data V2 entry".ToQuantity(cleared)}.");
            
            var inserted = _guildDataV2.InsertBulk(v1.Select(GuildDataV2.MigrateFromV1));
            
            Info(LogSource.Service, $"Migrated {"guild data entry".ToQuantity(inserted)} to V2.");

            return inserted;
        });
    }

    public GuildDataV2 GetData(ulong id)
    {
        return _guildDataV2.ValueLock(() =>
        {
            var conf = _guildDataV2.FindOne(g => g.Id == id);
            if (conf != null) return conf;
            var newConf = GuildDataV2.CreateFrom(_client.GetGuild(id));
            _guildDataV2.Insert(newConf);
            return newConf;
        });
    }

    public GuildDataV2 GetData(IGuild guild) => GetData(guild.Id);
    public HashSet<GuildDataV2> GetAllData() => _guildDataV2.ValueLock(() => _guildDataV2.FindAll().ToHashSet());

    public ValueTask<GuildDataV2> GetDataAsync(ulong id) => new(GetData(id));
    
    public void CreateGuildDataIfNotExists(IGuild guild) =>
        _guildDataV2.LockedRef(coll =>
        {
            if (!coll.Exists(g => g.Id == guild.Id))
                coll.Insert(GuildDataV2.CreateFrom(guild));
        });

    public void CreateGuildDataIfNotExists(ulong id) => CreateGuildDataIfNotExists(_client.GetGuild(id));

    public GuildDataV2 this[ulong id]
    {
        get => GetData(id);
        set => Save(value);
    }

    public void Modify(ulong guildId, DataEditor modifier)
    {
        var data = this[guildId];
        modifier(data);
        this[guildId] = data;
    }

    public void Save(GuildDataV2 newConfig)
    {
        _guildDataV2.LockedRef(coll =>
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
        
        data.Moderation.AddWarn(warn =>
        {
            warn.Issuer = issuer.Id;
            warn.Target = member.Id;
            warn.Reason = reason;
            warn.Date = DateTimeOffset.Now;
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