using System.IO;
using Newtonsoft.Json;

namespace SIVA
{
    public class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        public static BotConfig bot;

        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string Token;
        public string prefix;
        public bool debug;
        public string botGameToSet;
        public string twitchStreamer;
        public ulong botOwner;
        public uint defaultEmbedColour;
        public bool isSelfbot;
        public string currencySymbol;
        public ulong feedbackChannelId;
    }
}
