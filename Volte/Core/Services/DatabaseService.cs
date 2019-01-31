using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Discord;
using LiteDB;
using Volte.Core.Discord;
using Volte.Core.Data.Objects;
using Volte.Core.Data;

namespace Volte.Core.Services {
    public class DatabaseService {
        public Server GetConfig(IGuild guild) {
            return GetConfig(guild.Id);
        }
        public Server GetConfig(ulong id) {
            using (var db = new LiteDatabase(@"data/Volte.db")) {
                var conf = db.GetCollection<Server>("serverconfigs").FindOne(g => g.ServerId == id);
                if (conf == null) {
                    var newconf = Create(VolteBot.Client.GetGuild(id));
                    db.GetCollection<Server>("serverconfigs").Insert(newconf);
                    db.Dispose();
                    return newconf;
                }
                return conf;
                
            }
        }
        
        public void UpdateConfig(Server newConfig) {
            using (var db = new LiteDatabase(@"data/Volte.db")) {
                var collection = db.GetCollection<Server>("serverconfigs");
                collection.EnsureIndex(s => s.Id, true);
                collection.Update(newConfig);
            }
        }

        public DiscordUser GetUser(IUser user) {
            return GetUser(user.Id);
        }
        public DiscordUser GetUser(ulong id) {
            using (var db = new LiteDatabase("data/Volte.db")) {
                var user = db.GetCollection<DiscordUser>("users").FindOne(u => u.UserId == id);
                if (user == null) {
                    var newuser = Create(id);
                    db.GetCollection<DiscordUser>("users").Insert(newuser);
                    db.Dispose();
                    return newuser;
                }

                return user;
            }
        }

        public void UpdateUser(DiscordUser newUser) {
            using (var db = new LiteDatabase(@"data/Volte.db")) {
                var collection = db.GetCollection<DiscordUser>("users");
                collection.EnsureIndex(s => s.Id, true);
                collection.Update(newUser);
            }
        }

        private Server Create(IGuild guild) {
            return new Server {
                ServerId = guild.Id,
                GuildOwnerId = VolteBot.Client.GetGuild(guild.Id).OwnerId,
                Autorole = string.Empty,
                CommandPrefix = "$",
                WelcomeChannel = ulong.MinValue,
                WelcomeMessage = string.Empty,
                LeavingMessage = string.Empty,
                WelcomeColorR = 112,
                WelcomeColorG = 0,
                WelcomeColorB = 251,
                Antilink = false,
                VerifiedGuild = false,
                DeleteMessageOnCommand = false,
                ModRole = ulong.MinValue,
                AdminRole = ulong.MinValue,
                MassPingChecks = false
            };
        }
        
        public static DiscordUser Create(ulong id) {
            var u = VolteBot.Client.GetUser(id);
            var newUser = new DiscordUser {
                Tag = $"{u.Username}#{u.Discriminator}",
                UserId = id
            };
            return newUser;
        }

        
    }
}