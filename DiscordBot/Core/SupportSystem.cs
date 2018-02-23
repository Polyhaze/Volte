using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace DiscordBot.Core
{
    public class SupportSystem
    {
        static SupportSystem()
        {
            try
            {
                string jsonText = File.ReadAllText(filePath);
                supportConfig = JsonConvert.DeserializeObject<List<Support>>(jsonText);
            }
            catch (Exception)
            {
                SaveSupportConfig();
            }
        }

        private static List<Support> supportConfig = new List<Support>();
        private static string filePath = "Info/supportconfig.json";

        public static Support GetSupportConfig(ulong id)
        {
            var result = from a in supportConfig
                         where a.ServerId == id
                         select a;

            var config = result.FirstOrDefault();
            return config;
        }

        public static Support GetOrCreateSupportConfig(ulong id, string emoji, bool close, ulong sid)
        {
            var result = from a in supportConfig
                         where a.ServerId == id
                         select a;

            var config = result.FirstOrDefault();
            if (config == null) config = CreateSupportConfig(id, emoji, close, sid);
            return config;
        }

        public static void SaveSupportConfig()
        {
            string json = JsonConvert.SerializeObject(supportConfig, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Support CreateSupportConfig(ulong id, string emoji, bool close, ulong sid)
        {
            var newConfig = new Support()
            {
                ServerId = id,
                CanCloseOwnTicket = close,
                ReactionEmoji = emoji,
                SupportChannelId = sid,
            };
            supportConfig.Add(newConfig);
            SaveSupportConfig();
            return newConfig;

        }
    }
}
