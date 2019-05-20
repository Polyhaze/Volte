using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Volte.Data.Models;
using Volte.Core;
using Volte.Data.Models.Guild;

namespace Volte.Services
{
    [Service("Database", "The main Service for interacting with the Volte database.")]
    public sealed class DatabaseService
    {
        public static readonly LiteDatabase Database = new LiteDatabase("data/Volte.db");

        public GuildData GetData(IGuild guild) => GetData(guild.Id);

        public GuildData GetData(ulong id)
        {
            var coll = Database.GetCollection<GuildData>("serverconfigs");
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

        private GuildData Create(IGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = guild.OwnerId,
                Configuration = new GuildConfiguration
                {
                    Autorole = ulong.MinValue,
                    CommandPrefix = "$",
                    DeleteMessageOnCommand = false,
                    ModerationOptions = new ModerationOptions
                    {
                        AdminRole = ulong.MinValue,
                        Antilink = false,
                        Blacklist = new List<string>(),
                        MassPingChecks = false,
                        ModActionCaseNumber = ulong.MinValue,
                        ModActionLogChannel = ulong.MinValue,
                        ModRole = ulong.MinValue
                    },
                    WelcomeOptions = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = ulong.MinValue,
                        WelcomeColor = new Color(112, 0, 251).RawValue,
                        WelcomeMessage = string.Empty
                    }
                }
            };
    }
}