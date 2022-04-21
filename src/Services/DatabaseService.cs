using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using LiteDB;
using Volte;
using Volte.Entities;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Volte.Services
{
    public sealed class DatabaseService : IVolteService, IDisposable
    {
        public static readonly LiteDatabase Database = new LiteDatabase($"filename={Config.DataDirectory}/Volte.db;upgrade=true;connection=direct");

        private readonly DiscordShardedClient _client;

        private readonly ILiteCollection<GuildData> _guildData;
        private readonly ILiteCollection<Reminder> _reminderData;
        private readonly ILiteCollection<StarboardDbEntry> _starboardData;

        public DatabaseService(DiscordShardedClient discordShardedClient)
        {
            _client = discordShardedClient;
            _guildData = Database.GetCollection<GuildData>("guilds");
            _reminderData = Database.GetCollection<Reminder>("reminders");
            _starboardData = Database.GetCollection<StarboardDbEntry>("starboard");
            _starboardData.EnsureIndex("composite_id", $"$.{nameof(StarboardDbEntry.GuildId)} + '_' + $.{nameof(StarboardDbEntry.Key)}");
        }
        
        public HashSet<Reminder> AllReminders => _reminderData.ValueLock(() => _reminderData.FindAll().ToHashSet());
        public HashSet<GuildData> AllGuilds => _guildData.ValueLock(() => _guildData.FindAll().Where(x => _client.GetGuild(x.Id) != null).ToHashSet());


        public GuildData GetData(SocketGuild guild) => GetData(guild?.Id);

        public ValueTask<GuildData> GetDataAsync(ulong id) => new ValueTask<GuildData>(GetData(id));

        public GuildData GetData(ulong? id)
        {
            if (id is null) return null;
            return _guildData.ValueLock(() =>
            {
                var conf = _guildData.FindOne(g => g.Id == id);
                if (conf != null) return conf;
                var newConf = GuildData.CreateFrom(_client.GetGuild(id.Value));
                _guildData.Insert(newConf);
                return newConf;
            });
        }

        public Reminder GetReminder(long databaseId) => AllReminders.FirstOrDefault(x => x.Id == databaseId);

        public HashSet<Reminder> GetReminders(IUser user) => GetReminders(user.Id).ToHashSet();

        public HashSet<Reminder> GetReminders(ulong creator)
            => AllReminders.Where(r => r.CreatorId == creator).ToHashSet();

        public bool TryDeleteReminder(Reminder reminder) => _reminderData.ValueLock(() => _reminderData.Delete(reminder.Id));

        public void CreateReminder(Reminder reminder) => _reminderData.ValueLock(() => _reminderData.Insert(reminder));

        public void Modify(ulong guildId, Action<GuildData> modifier)
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
                _guildData.EnsureIndex(s => s.Id, true);
                _guildData.Update(newConfig);
            });
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
}