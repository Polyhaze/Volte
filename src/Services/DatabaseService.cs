using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using LiteDB;
using Volte.Core;
using Volte.Data;
using Volte.Data.Models.Guild;

namespace Volte.Services
{
    [Service("Database", "The main Service for interacting with the Volte database.")]
    public sealed class DatabaseService
    {
        public static DatabaseService Instance = VolteBot.GetRequiredService<DatabaseService>();
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        public GuildData GetData(IGuild guild) => GetData(guild.Id);

        public GuildData GetData(ulong id)
        {
            var coll = Database.GetCollection<GuildData>("guilds");
            var conf = coll.FindOne(g => g.Id == id);
            if (!(conf is null)) return conf;
            var newConf = Create(VolteBot.Client.GetGuild(id));
            coll.Insert(newConf);
            return newConf;
        }

        public void UpdateData(GuildData newConfig)
        {
            var collection = Database.GetCollection<GuildData>("guilds");
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newConfig);
        }

        public Task MigrateAsync()
        {
            var oldDb = new LiteDatabase("data/Volte-old.db");
            var collection = oldDb.GetCollection<DummyGuildConfig>("serverconfigs");
            var newColl = Database.GetCollection<GuildData>("guilds");
            foreach (var record in collection.FindAll())
            {
                var @new = new GuildData
                {
                    Id = record.ServerId,
                    OwnerId = record.GuildOwnerId,
                    Configuration = new GuildConfiguration
                    {
                        Autorole = VolteBot.Client.GetGuild(record.ServerId)?.Roles
                                       .FirstOrDefault(x => x.Name.EqualsIgnoreCase(record.Autorole))?.Id ?? ulong.MinValue,
                        CommandPrefix = record.CommandPrefix ?? Config.CommandPrefix,
                        DeleteMessageOnCommand = record.DeleteMessageOnCommand,
                        Moderation = new ModerationOptions
                        {
                            AdminRole = record.ModerationOptions.AdminRole,
                            Antilink = record.ModerationOptions.Antilink,
                            Blacklist = record.ModerationOptions.Blacklist ?? new List<string>(),
                            MassPingChecks = record.ModerationOptions.MassPingChecks,
                            ModActionLogChannel = record.ModerationOptions.ModActionLogChannel,
                            ModRole = record.ModerationOptions.ModRole
                        },
                        Welcome = new WelcomeOptions
                        {
                            LeavingMessage = record.WelcomeOptions.LeavingMessage ?? string.Empty,
                            WelcomeChannel = record.WelcomeOptions.WelcomeChannel,
                            WelcomeColor = new Color(record.WelcomeOptions.WelcomeColorR, record.WelcomeOptions.WelcomeColorG, record.WelcomeOptions.WelcomeColorB).RawValue,
                            WelcomeMessage = record.WelcomeOptions.WelcomeMessage ?? string.Empty
                        }
                    },
                    Extras = new GuildExtras
                    {
                        ModActionCaseNumber = record.ModerationOptions.ModActionCaseNumber,
                        SelfRoles = record.SelfRoles ?? new List<string>(),
                        Tags = record.Tags ?? new List<Tag>(),
                        Warns = record.Warns ?? new List<Warn>()
                    }
                };

                newColl.EnsureIndex(x => x.Id, true);
                newColl.Upsert(@new);

            }
            
            return Task.CompletedTask;
        }

        private GuildData Create(IGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = guild.OwnerId,
                Configuration = new GuildConfiguration
                {
                    Autorole = ulong.MinValue,
                    CommandPrefix = Config.CommandPrefix,
                    DeleteMessageOnCommand = false,
                    Moderation = new ModerationOptions
                    {
                        AdminRole = ulong.MinValue,
                        Antilink = false,
                        Blacklist = new List<string>(),
                        MassPingChecks = false,
                        ModActionLogChannel = ulong.MinValue,
                        ModRole = ulong.MinValue
                    },
                    Welcome = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = ulong.MinValue,
                        WelcomeColor = new Color(112, 0, 251).RawValue,
                        WelcomeMessage = string.Empty
                    }
                },
                Extras = new GuildExtras
                {
                    ModActionCaseNumber = ulong.MinValue,
                    SelfRoles = new List<string>(),
                    Tags = new List<Tag>(),
                    Warns = new List<Warn>()
                }
            };
    }
}