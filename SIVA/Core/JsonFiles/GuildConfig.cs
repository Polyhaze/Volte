using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace SIVA.Core.JsonFiles
{
    public class Guild
    {
        #region JSONValueDeclaration

        public Guild()
        {
            AntilinkIgnoredChannels = new List<ulong>();
            SelfRoles = new List<string>();
            Blacklist = new List<string>();
            CustomCommands = new Dictionary<string, string>();
        }

        public ulong ServerId { get; set; }
        public bool CanCloseOwnTicket { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string SupportChannelName { get; set; }
        public ulong SupportChannelId { get; set; }
        public string SupportRole { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public bool Leveling { get; set; }
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColour1 { get; set; }
        public int WelcomeColour2 { get; set; }
        public int WelcomeColour3 { get; set; }
        public int EmbedColour1 { get; set; }
        public int EmbedColour2 { get; set; }
        public int EmbedColour3 { get; set; }
        public bool MassPengChecks { get; set; }
        public bool Antilink { get; set; }
        public bool VerifiedGuild { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public bool IsTodEnabled { get; set; }
        public ulong ServerLoggingChannel { get; set; }
        public bool IsServerLoggingEnabled { get; set; }
        public List<ulong> AntilinkIgnoredChannels { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<string> Blacklist { get; set; }
        public Dictionary<string, string> CustomCommands { get; set; }

        #endregion
    }

    public static class GuildConfig
    {
        private static readonly List<Guild> Config = new List<Guild>();
        private static readonly string filePath = "data/GuildConfigs.json";

        static GuildConfig()
        {
            try
            {
                var jsonText = File.ReadAllText(filePath);
                Config = JsonConvert.DeserializeObject<List<Guild>>(jsonText);
            }
            catch (Exception)
            {
                SaveGuildConfig();
            }
        }

        public static Guild GetGuildConfig(ulong id)
        {
            var result = Config.FirstOrDefault(x => x.ServerId == id);

            var config = result;
            return config;
        }

        public static Guild GetOrCreateConfig(ulong id)
        {
            var result = Config.FirstOrDefault(x => x.ServerId == id);

            var config = result ?? CreateGuildConfig(id);
            return config;
        }

        public static void SaveGuildConfig()
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Guild CreateGuildConfig(ulong id)
        {
            var newconfig = new Guild
            {
                ServerId = id,
                GuildOwnerId = 0,
                Autorole = "",
                SupportChannelName = "",
                SupportRole = "Support",
                CanCloseOwnTicket = true,
                SupportChannelId = 00000000000000,
                CommandPrefix = "$",
                WelcomeChannel = 0,
                WelcomeMessage = string.Empty,
                LeavingMessage = string.Empty,
                WelcomeColour1 = 112,
                WelcomeColour2 = 0,
                WelcomeColour3 = 251,
                EmbedColour1 = 112,
                EmbedColour2 = 0,
                EmbedColour3 = 251,
                Antilink = false,
                VerifiedGuild = false
            };
            Config.Add(newconfig);
            SaveGuildConfig();
            return newconfig;
        }
    }
}