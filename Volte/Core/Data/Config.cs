using System;
using System.IO;
using Newtonsoft.Json;

namespace Volte.Core.Data {
    public class Config {
        private static readonly BotConfig Bot;
        private const string ConfigFile = "data/config.json";

        static Config() {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!File.Exists(ConfigFile)) {
                Bot = new BotConfig {
                    Token = "token here",
                    CommandPrefix = "$",
                    Owner = 0,
                    Game = "in Volte V2 Code!",
                    Streamer = "GreemDev",
                    SuccessEmbedColor = 0x7000FB,
                    ErrorEmbedColor = 0xFF0000,
                    LogAllCommands = true,
                    BlacklistedServerOwners = new ulong[]{}
                };
                var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            else {
                var json = File.ReadAllText(ConfigFile);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static string GetToken() => Bot.Token;
        public static string GetCommandPrefix() => Bot.CommandPrefix;
        public static ulong GetOwner() => Bot.Owner;
        public static string GetGame() => Bot.Game;
        public static string GetStreamer()=> Bot.Streamer;
        public static uint GetSuccessColor() => Bot.SuccessEmbedColor;
        public static uint GetErrorColour() => Bot.ErrorEmbedColor;
        public static bool GetLogAllCommands() => Bot.LogAllCommands;
        public static ulong[] GetBlacklistedOwners() => Bot.BlacklistedServerOwners;

        private struct BotConfig {
            public string Token;
            public string CommandPrefix;
            public ulong Owner;
            public string Game;
            public string Streamer;
            public uint SuccessEmbedColor;
            public uint ErrorEmbedColor;
            public bool LogAllCommands;
            public ulong[] BlacklistedServerOwners;
        }
    }
}