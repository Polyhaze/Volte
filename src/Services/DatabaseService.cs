using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Volte.Core;
using Volte.Core.Models.Guild;

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

        public GuildData GetData(SocketGuild guild) => GetData(guild.Id);

        public GuildData GetData(ulong id)
        {
            var coll = Database.GetCollection<GuildData>("guilds");
            var conf = coll.FindOne(g => g.Id == id);
            if (!(conf is null)) return conf;
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

        private static GuildData Create(SocketGuild guild)
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
                        Blacklist = new List<string>(),
                        MassPingChecks = default,
                        ModActionLogChannel = default,
                        ModRole = default,
                        CheckAccountAge = false,
                        VerifiedRole = default,
                        UnverifiedRole = default
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
                    SelfRoles = new List<string>(),
                    Tags = new List<Tag>(),
                    Warns = new List<Warn>()
                }
            };

        public void Dispose() 
            => Database.Dispose();
    }
}