using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace DiscordBot.Core.UserAccounts
{
    public static class Modlogs
    {
        static Modlogs()
        {
            try
            {
                string jsonText = File.ReadAllText(filePath);
                mlconfig = JsonConvert.DeserializeObject<List<Modlog>>(jsonText);
            }
            catch (Exception)
            {
                SaveModlogConfig();
            }
        }

        public static List<Modlog> mlconfig = new List<Modlog>();
        private static string filePath = "Info/modlogs.json";

        public static void SaveModlogConfig()
        {
            string json = JsonConvert.SerializeObject(mlconfig, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Modlog CreateModlogConfig(ulong id, ulong channel)
        {
            var newConfig = new Modlog()
            {
                serverId = id,
                channelId = channel
            };
            mlconfig.Add(newConfig);
            SaveModlogConfig();
            return newConfig;

        }

        public static Modlog GetModlogConfig(ulong id)
        {
            var result = from a in mlconfig
                         where a.serverId == id
                         select a;

            var config = result.FirstOrDefault();
            return config;
        }
    }
}
