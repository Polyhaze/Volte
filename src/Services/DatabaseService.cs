using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.Guild;

namespace Volte.Services
{
    public sealed class DatabaseService : VolteService, IDisposable
    {
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        private readonly DiscordShardedClient _client;
        private readonly LoggingService _logger;

        public DatabaseService(DiscordShardedClient discordShardedClient,
            LoggingService loggingService)
        {
            _client = discordShardedClient;
            _logger = loggingService;
        }

        public GuildData GetData(SocketGuild guild) => GetData(guild.Id);

        public GuildData GetData(ulong id)
        {
            _logger.Debug(LogSource.Volte, $"Getting data for guild {id}.");
            var coll = Database.GetCollection<GuildData>("guilds");
            var conf = coll.FindOne(g => g.Id == id);
            if (!(conf is null)) return conf;
            var newConf = Create(_client.GetGuild(id));
            coll.Insert(newConf);
            return newConf;
        }

        public void UpdateData(GuildData newConfig)
        {
            _logger.Debug(LogSource.Volte, $"Updating data for guild {newConfig.Id}");
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