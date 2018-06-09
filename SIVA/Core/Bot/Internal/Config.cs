using System.IO;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

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
                var json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFile, json);
            }
            else
            {
                var json = File.ReadAllText(configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static BotConfig GetOrCreateConfig()
        {
            if (bot.Token == "")
            {
                var jsonFile = JsonConvert.SerializeObject(bot, Formatting.Indented);
                bot = JsonConvert.DeserializeObject<BotConfig>(jsonFile);
                File.WriteAllText(configFile, jsonFile);
                return bot;
            }

            return bot;
        }

        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(bot, Formatting.Indented);
            File.WriteAllText(configFile, json);
        }

        public struct BotConfig
        {
            public string Token;
            public string DblToken;
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