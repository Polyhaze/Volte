using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Volte.Core;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class DatabaseService : VolteService, IDisposable
    {
        public static readonly LiteDatabase Database = new LiteDatabase($"filename={Config.DataDirectory}/Volte.db;upgrade=true;connection=direct");
        private const string GuildsCollection = "guilds";
        private const string RemindersCollection = "reminders";

        private readonly DiscordShardedClient _client;

        public DatabaseService(DiscordShardedClient discordShardedClient)
        {
            _client = discordShardedClient;
        }

        public GuildData GetData(SocketGuild guild) => GetData(guild.Id);

        public ValueTask<GuildData> GetDataAsync(ulong id) => new ValueTask<GuildData>(GetData(id));

        public GuildData GetData(ulong id)
        {
            var coll = Database.GetCollection<GuildData>(GuildsCollection);
            var conf = coll.FindOne(g => g.Id == id);
            if (conf != null) return conf;
            var newConf = Create(_client.GetGuild(id));
            coll.Insert(newConf);
            return newConf;
        }

        public HashSet<Reminder> GetReminders(IUser user, IGuild guild = null) => GetReminders(user.Id, guild?.Id ?? 0).ToHashSet();

        public HashSet<Reminder> GetReminders(ulong id, ulong guild = 0) 
            => GetAllReminders().Where(r => r.CreatorId == id && (guild is 0 || r.GuildId == guild)).ToHashSet();

        public bool TryDeleteReminder(Reminder reminder) => Database.GetCollection<Reminder>(RemindersCollection).Delete(reminder.Id);

        public HashSet<Reminder> GetAllReminders() => Database.GetCollection<Reminder>(RemindersCollection).FindAll().ToHashSet();
        
        public void CreateReminder(Reminder reminder) => Database.GetCollection<Reminder>(RemindersCollection).Insert(reminder);

        public void Modify(ulong guildId, DataEditor modifier)
        {
            var data = GetData(guildId);
            modifier(data);
            Save(data);
        }

        public void Save(GuildData newConfig)
        {
            var collection = Database.GetCollection<GuildData>(GuildsCollection);
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newConfig);
        }

        private static GuildData Create(IGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = guild.OwnerId,
                Configuration = new GuildConfiguration
                {
                    Autorole = default,
                    CommandPrefix = Config.CommandPrefix,
                    Moderation = new ModerationOptions
                    {
                        AdminRole = default,
                        Antilink = default,
                        Blacklist = new HashSet<string>(),
                        MassPingChecks = default,
                        ModActionLogChannel = default,
                        ModRole = default,
                        CheckAccountAge = false,
                        VerifiedRole = default,
                        UnverifiedRole = default,
                        BlacklistAction = BlacklistAction.Nothing,
                        ShowResponsibleModerator = true
                    },
                    Welcome = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = default,
                        WelcomeColor = new Color(0x7000FB).RawValue,
                        WelcomeMessage = string.Empty
                    }
                },
                Extras = new GuildExtras
                {
                    ModActionCaseNumber = default,
                    SelfRoles = new HashSet<string>(),
                    Tags = new HashSet<Tag>(),
                    Warns = new HashSet<Warn>()
                }
            };

        public void Dispose() 
            => Database.Dispose();
    }
}