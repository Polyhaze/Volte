using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SIVA.Core.Config
{
    public class GuildConfig
    {
        static GuildConfig()
        {
            try
            {
                var jsonText = File.ReadAllText(filePath);
                Config = JsonConvert.DeserializeObject<List<Guild>>(jsonText);
            }
            catch(Exception)
            {
                SaveGuildConfig();
            }
        }

        private static readonly List<Guild> Config = new List<Guild>();
        private static string filePath = "Resources/GuildConfigs.json";

        public static Guild GetGuildConfig(ulong id)
        {
            var result = from a in Config
                where a.ServerId == id
                select a;

            var config = result.FirstOrDefault();
            return config;
        }

        public static Guild GetOrCreateConfig(ulong id)
        {
            var result = from a in Config
                where a.ServerId == id
                select a;

            var config = result.FirstOrDefault() ?? CreateGuildConfig(id);
            return config;
        }

        public static void SaveGuildConfig()
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Guild CreateGuildConfig(ulong id)
        {
            var newConfig = new Guild()
            {
                ServerId = id,
                RoleToApply = "",
                SupportCategoryId = 000000000000000,
                SupportChannelName = "",
                SupportRole = "",
                CanCloseOwnTicket = true,
                ReactionEmoji = "",
                ChannelId = 0000000000000,
                SupportChannelId = 00000000000000
                
            };
            Config.Add(newConfig);
            SaveGuildConfig();
            return newConfig;

        }


    }
}
