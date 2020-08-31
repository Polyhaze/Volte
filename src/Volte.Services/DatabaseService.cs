using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using LiteDB;
using Volte.Core;
using Volte.Core.Helpers;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class DatabaseService : VolteService, IDisposable
    {
        public static readonly LiteDatabase Database = new LiteDatabase("filename=data/Volte.db;upgrade=true;connection=direct");

        private readonly DiscordShardedClient _client;

        public DatabaseService(DiscordShardedClient discordShardedClient)
        {
            _client = discordShardedClient;
        }

        public void ModifyAndSaveData(ulong id, Func<GuildData, GuildData> func)
        {
            var d = GetData(id);
            d = func(d);
            UpdateData(d);
        }
        
        public async Task ModifyAndSaveDataAsync(ulong id, Func<GuildData, Task<GuildData>> func)
        {
            var d = GetData(id);
            d = await func(d);
            UpdateData(d);
        }

        public GuildData GetData(DiscordGuild guild) => GetData(guild.Id);

        public GuildData GetData(ulong id)
        {
            var coll = Database.GetCollection<GuildData>("guilds");
            var conf = coll.FindOne(g => (ulong)g.Id == (ulong)id);
            if (conf is not null) return conf;
            var newConf = Create(_client.GetGuild(id));
            coll.Insert(newConf);
            return newConf;
        }

        public void UpdateData(GuildData newConfig)
        {
            var collection = Database.GetCollection<GuildData>("guilds");
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newConfig);
        }

        private static GuildData Create(DiscordGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = DiscordReflectionHelper.GetOwnerId(guild),
                Configuration = new GuildConfiguration
                {
                    Autorole = default,
                    CommandPrefix = Config.CommandPrefix,
                    DeleteMessageOnCommand = default,
                    Moderation = new ModerationOptions
                    {
                        AdminRole = default,
                        Antilink = default,
                        Blacklist = new List<string>(),
                        MassPingChecks = default,
                        ModActionLogChannel = default,
                        ModRole = default
                    },
                    Welcome = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = default,
                        WelcomeColor = new DiscordColor(0x7000FB).Value,
                        WelcomeMessage = string.Empty
                    },
                    Starboard = new StarboardOptions
                    {
                        Enabled = false,
                        StarboardChannel = default,
                        StarsRequiredToPost = 1
                    }
                },
                Extras = new GuildExtras
                {
                    ModActionCaseNumber = default,
                    SelfRoles = new List<string>(),
                    StarboardedMessages = new ConcurrentDictionary<ulong, StarboardEntry>(),
                    Tags = new List<Tag>(),
                    Warns = new List<Warn>()
                }
            };

        public void Dispose() 
            => Database.Dispose();
    }
}