using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using SIVA.Core.Runtime;

namespace SIVA.Core.Files
{
    public static class ServerConfig
    {
        private static readonly List<Server> Config = new List<Server>();
        private const string FilePath = "data/serverconfigs.json";

        static ServerConfig()
        {
            try
            {
                var jText = File.ReadAllText(FilePath);
                Config = JsonConvert.DeserializeObject<List<Server>>(jText);
            }
            catch (Exception)
            {
                Save();
            }
        }

        public static Server GetOrCreate(ulong id)
        {
            return Config.FirstOrDefault(x => x.ServerId == id) ?? Create(id);
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static Server Create(ulong id)
        {
            var newConf = new Server
            {
                ServerId = id,
                GuildOwnerId = Program.Client.GetGuild(id).OwnerId,
                Autorole = string.Empty,
                SupportChannelName = string.Empty,
                SupportRole = "Support",
                CanCloseOwnTicket = true,
                SupportChannelId = ulong.MinValue,
                CommandPrefix = "$",
                WelcomeChannel = ulong.MinValue,
                WelcomeMessage = string.Empty,
                LeavingMessage = string.Empty,
                WelcomeColourR = 112,
                WelcomeColourG = 0,
                WelcomeColourB = 251,
                EmbedColourR = 112,
                EmbedColourG = 0,
                EmbedColourB = 251,
                Antilink = false,
                VerifiedGuild = false
            };
            Config.Add(newConf);
            Save();
            return newConf;
        }
        
        
    }

    public class Server
    {
        #region JsonValueDeclaration

        public Server()
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
        public int WelcomeColourR { get; set; }
        public int WelcomeColourG { get; set; }
        public int WelcomeColourB { get; set; }
        public int EmbedColourR { get; set; }
        public int EmbedColourG { get; set; }
        public int EmbedColourB { get; set; }
        public bool MassPengChecks { get; set; }
        public bool Antilink { get; set; }
        public bool VerifiedGuild { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public bool IsTodEnabled { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public ulong ServerLoggingChannel { get; set; }
        public bool IsServerLoggingEnabled { get; set; }
        public List<ulong> AntilinkIgnoredChannels { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<string> Blacklist { get; set; }
        public Dictionary<string, string> CustomCommands { get; set; }

        #endregion
    }
}