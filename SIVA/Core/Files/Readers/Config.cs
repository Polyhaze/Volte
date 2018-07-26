using System.IO;
using Newtonsoft.Json;

namespace SIVA.Core.Files.Readers {
    public class Config {
        private static readonly BotConfig Bot;
        private const string ConfigFile = "data/config.json";

        static Config() {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!File.Exists(ConfigFile)) {
                Bot = new BotConfig();
                var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            else {
                var json = File.ReadAllText(ConfigFile);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static string GetToken() {
            return Bot.Token;
        }

        public static string GetDblToken() {
            return Bot.DblToken;
        }

        public static string GetCommandPrefix() {
            return Bot.CommandPrefix;
        }

        public static ulong GetOwner() {
            return Bot.Owner;
        }

        public static string GetGame() {
            return Bot.Game;
        }

        public static string GetStreamer() {
            return Bot.Streamer;
        }

        public static uint GetErrorColour() {
            return Bot.ErrorEmbedColour;
        }

        public static bool GetLogAllCommands() {
            return Bot.LogAllCommands;
        }

        public static ulong[] GetBlacklistedOwners() {
            return Bot.BlacklistedServerOwners;
        }

        private struct BotConfig {
            public string Token;
            public string DblToken;
            public string CommandPrefix;
            public ulong Owner;
            public string Game;
            public string Streamer;
            public uint ErrorEmbedColour;
            public bool LogAllCommands;
            public ulong[] BlacklistedServerOwners;
        }
    }
}