using System.IO;
using Newtonsoft.Json;

namespace SIVA.Core.Files.Readers
{
    public class Config
    {
        public static BotConfig conf;
        private const string ConfigFile = "data/config.json";

        static Config()
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!File.Exists(ConfigFile))
            {
                conf = new BotConfig();
                var json = JsonConvert.SerializeObject(conf, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigFile);
                conf = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
        
        
        public struct BotConfig
        {
            public string Token;
            public string DblToken;
            public string CommandPrefix;
            public string Game;
            public string Streamer;
            public uint ErrorEmbedColour;
            public bool LogAllCommands;
            public ulong[] BlacklistedServerOwners;
        }
    }
}