using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using LiteDB;
using Volte.Core.Data;
using Volte.Core.Data.Models.Guild;

namespace Volte.Services
{
    [Service("Database", "The main Service for interacting with the Volte database.")]
    public sealed class DatabaseService
    {
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        private DiscordShardedClient _client;

        public DatabaseService(DiscordShardedClient DiscordShardedClient)
        {
            _client = DiscordShardedClient;
        }

        public GuildData GetData(IGuild guild) => GetData(guild.Id);

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