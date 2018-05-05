using System.IO;
using Newtonsoft.Json;
using SIVA.Core.Bot.Services;

namespace SIVA.Core.Bot.Internal
{
    public class Config
    {
        private const string configFile = "data/BotConfig.json";
        public static BotConfig bot;

        static Config()
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!File.Exists(configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static BotConfig GetOrCreateConfig()
        {
            if (bot.Token == "")
            {
                string jsonFile = JsonConvert.SerializeObject(bot, Formatting.Indented);
                bot = JsonConvert.DeserializeObject<BotConfig>(jsonFile);
                File.WriteAllText(configFile, jsonFile);
                return bot;
            }
            else
            {
                return bot;
            }
        }

        public static void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
            File.WriteAllText(configFile, json);
        }

        public struct BotConfig
        {
            public string Token;
            public string Prefix;
            public bool Debug;
            public string BotGameToSet;
            public string TwitchStreamer;
            public ulong BotOwner;
            public uint DefaultEmbedColour;
            public bool IsSelfbot;
            public string CurrencySymbol;
            public ulong FeedbackChannelId;
            public uint ErrorEmbedColour;
            public string LogSeverity;
            public ulong[] Blacklist;
        }
    }
}

