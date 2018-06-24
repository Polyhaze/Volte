using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIVA.Core.Files.Objects;
using Newtonsoft.Json;
using SIVA.Core.Runtime;

namespace SIVA.Core.Files.Readers
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
}