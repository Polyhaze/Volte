using Discord;
using Discord.WebSocket;
using LiteDB;
using Volte.Data.Objects;
using Volte.Discord;

namespace Volte.Services
{
    [Service("Database", "The main Service for interacting with the Volte database.")]
    public sealed class DatabaseService
    {
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        public GuildConfiguration GetConfig(IGuild guild)
        {
            return GetConfig(guild.Id);
        }

        public GuildConfiguration GetConfig(ulong id)
        {
            var coll = Database.GetCollection<GuildConfiguration>("serverconfigs");
            var conf = coll.FindOne(g => g.ServerId == id);
            if (!(conf is null)) return conf;
            var newConf = Create(VolteBot.Client.GetGuild(id));
            coll.Insert(newConf);
            return newConf;

        }

        public void UpdateConfig(GuildConfiguration newConfig)
        {
            var collection = Database.GetCollection<GuildConfiguration>("serverconfigs");
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newConfig);
        }

        private GuildConfiguration Create(SocketGuild guild)
        {
            return new GuildConfiguration
            {
                ServerId = guild.Id,
                GuildOwnerId = guild.OwnerId,
                Autorole = string.Empty,
                CommandPrefix = "$",
                DeleteMessageOnCommand = false,
                WelcomeOptions = new WelcomeOptions
                {
                    WelcomeChannel = ulong.MinValue,
                    WelcomeMessage = string.Empty,
                    WelcomeColorR = 112,
                    WelcomeColorG = 0,
                    WelcomeColorB = 251
                },
                ModerationOptions = new ModerationOptions
                {
                    ModRole = ulong.MinValue,
                    AdminRole = ulong.MinValue,
                    MassPingChecks = false,
                    Antilink = false
                }
            };
        }
    }
}