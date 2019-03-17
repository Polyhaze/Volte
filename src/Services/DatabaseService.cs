using System.Linq;
using DSharpPlus.Entities;
using LiteDB;
using Volte.Data.Objects;
using Volte.Discord;

namespace Volte.Services
{
    [Service("Database", "The main Service for interacting with the Volte database.")]
    public sealed class DatabaseService
    {
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        public DiscordServer GetConfig(DiscordGuild guild)
        {
            return GetConfig(guild.Id);
        }

        public DiscordServer GetConfig(ulong id)
        {
            var coll = Database.GetCollection<DiscordServer>("serverconfigs");
            var conf = coll.FindOne(g => g.ServerId == id);
            if (conf is null)
            {
                var newConf = Create(VolteBot.Client.Guilds.First(x => x.Value.Id == id).Value);
                coll.Insert(newConf);
                return newConf;
            }

            return conf;
        }

        public void UpdateConfig(DiscordServer newConfig)
        {
            var collection = Database.GetCollection<DiscordServer>("serverconfigs");
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newConfig);
        }

        private DiscordServer Create(DiscordGuild guild)
        {
            return new DiscordServer
            {
                ServerId = guild.Id,
                GuildOwnerId = guild.Owner.Id,
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
                },
                VerificationOptions = new VerificationOptions
                {
                    Enabled = false,
                    MessageId = ulong.MinValue,
                    RoleId = ulong.MinValue
                }
            };
        }
    }
}