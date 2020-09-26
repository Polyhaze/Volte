using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using JetBrains.Annotations;
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
        private readonly ILiteCollection<GuildData> _guildData;
        private readonly ILiteCollection<StarboardEntryBase> _starboardData;

        public DatabaseService(DiscordShardedClient discordShardedClient)
        {
            _client = discordShardedClient;
            
            _guildData = Database.GetCollection<GuildData>("guilds");
            _guildData.EnsureIndex(s => s.Id, true);
            
            _starboardData = Database.GetCollection<StarboardEntryBase>("starboard");
            _starboardData.EnsureIndex("composite_id", $"$.{nameof(StarboardEntryBase.GuildId)} + '_' + $.{nameof(StarboardEntryBase.Key)}");
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
            var conf = _guildData.FindOne(g => (ulong)g.Id == (ulong)id);
            if (conf is not null) return conf;
            var newConf = CreateData(_client.GetGuild(id));
            _guildData.Insert(newConf);
            return newConf;
        }

        public void UpdateData(GuildData newConfig)
        {
            _guildData.Update(newConfig);
        }

        private static GuildData CreateData(DiscordGuild guild)
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
                    Tags = new List<Tag>(),
                    Warns = new List<Warn>()
                }
            };

        [CanBeNull]
        private StarboardEntryBase GetStargazersInternal(ulong guildId, ulong messageId)
        {
            return _starboardData.FindOne(g => g.GuildId == guildId && g.Key == messageId);
        }

        [CanBeNull]
        public StarboardEntry2 GetStargazers(ulong guildId, ulong messageId)
        {
            return GetStargazersInternal(guildId, messageId)?.Value;
        }

        public bool TryGetStargazers(ulong guildId, ulong messageId, [NotNullWhen(true)] out StarboardEntry2 entry)
        {
            entry = GetStargazersInternal(guildId, messageId)?.Value;
            return entry != null;
        }

        public void UpdateStargazers(StarboardEntry2 entry)
        {
            _starboardData.Upsert($"{entry.GuildId}_{entry.StarboardMessageId}", new StarboardEntryBase
            {
                GuildId = entry.GuildId,
                Key = entry.StarboardMessageId,
                Value = entry
            });

            _starboardData.Upsert($"{entry.GuildId}_{entry.StarredMessageId}", new StarboardEntryBase
            {
                GuildId = entry.GuildId,
                Key = entry.StarredMessageId,
                Value = entry
            });
        }

        public void RemoveStargazers(StarboardEntry2 entry)
        {
            _starboardData.Delete($"{entry.GuildId}_{entry.StarboardMessageId}");
            _starboardData.Delete($"{entry.GuildId}_{entry.StarredMessageId}");
        }

        public void Dispose() 
            => Database.Dispose();
    }
}