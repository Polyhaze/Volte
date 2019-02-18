using Discord;
using LiteDB;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;

namespace Volte.Core.Services
{
    public class DatabaseService
    {
        private static readonly LiteDatabase Database = new LiteDatabase(@"data/Volte.db");

        public DiscordServer GetConfig(IGuild guild)
        {
            return GetConfig(guild.Id);
        }

        public DiscordServer GetConfig(ulong id)
        {
            var conf = Database.GetCollection<DiscordServer>("serverconfigs").FindOne(g => g.ServerId == id);
            if (conf is null)
            {
                var newConf = Create(VolteBot.Client.GetGuild(id));
                Database.GetCollection<DiscordServer>("serverconfigs").Insert(newConf);
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

        public DiscordUser GetUser(IUser user)
        {
            return GetUser(user.Id);
        }

        public DiscordUser GetUser(ulong id)
        {
            var user = Database.GetCollection<DiscordUser>("users").FindOne(u => u.UserId == id);
            if (user is null)
            {
                var newUser = Create(id);
                Database.GetCollection<DiscordUser>("users").Insert(newUser);
                return newUser;
            }

            return user;
        }

        public void UpdateUser(DiscordUser newUser)
        {
            var collection = Database.GetCollection<DiscordUser>("users");
            collection.EnsureIndex(s => s.Id, true);
            collection.Update(newUser);
        }

        private DiscordServer Create(IGuild guild)
        {
            return new DiscordServer
            {
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
                DeleteMessageOnCommand = false,
                ModRole = ulong.MinValue,
                AdminRole = ulong.MinValue,
                MassPingChecks = false
            };
        }

        public static DiscordUser Create(ulong id)
        {
            var u = VolteBot.Client.GetUser(id);
            var newUser = new DiscordUser
            {
                Tag = $"{u.Username}#{u.Discriminator}",
                UserId = id
            };
            return newUser;
        }
    }
}