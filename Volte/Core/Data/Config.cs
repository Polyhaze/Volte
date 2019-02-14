using System;
using System.IO;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Runtime;
using Volte.Core.Services;

namespace Volte.Core.Data {
    public class Config {
        private static BotConfig _bot;
        private const string ConfigFile = "data/config.json";

        static Config() {
            CreateIfNotExists();
            if (File.Exists(ConfigFile) && !string.IsNullOrEmpty(File.ReadAllText(ConfigFile)))
                _bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(ConfigFile));
        }

        public static void CreateIfNotExists() {
            if (File.Exists(ConfigFile) && !string.IsNullOrEmpty(File.ReadAllText(ConfigFile))) return;
            var logger = VolteBot.ServiceProvider.GetRequiredService<LoggingService>();
            logger.Log(LogSeverity.Warning, LogSource.Volte,
                "config.json didn't exist or was empty. Created it for you.").GetAwaiter().GetResult();
            _bot = new BotConfig {
                Token = "token here",
                CommandPrefix = "$",
                Owner = 0,
                Game = "in Volte V2 Code!",
                Streamer = "streamer here",
                SuccessEmbedColor = 0x7000FB,
                ErrorEmbedColor = 0xFF0000,
                LogAllCommands = true,
                JoinLeaveLog = new JoinLeaveLog(),
                BlacklistedServerOwners = new ulong[] { }
            };
            File.WriteAllText(ConfigFile,
                JsonConvert.SerializeObject(_bot, Formatting.Indented));
        }

        public static string GetToken() => _bot.Token;
        public static string GetCommandPrefix() => _bot.CommandPrefix;
        public static ulong GetOwner() => _bot.Owner;
        public static string GetGame() => _bot.Game;
        public static string GetStreamer() => _bot.Streamer;
        public static uint GetSuccessColor() => _bot.SuccessEmbedColor;
        public static uint GetErrorColor() => _bot.ErrorEmbedColor;
        public static bool GetLogAllCommands() => _bot.LogAllCommands;
        public static JoinLeaveLog GetJoinLeaveLog() => _bot.JoinLeaveLog;
        public static ulong[] GetBlacklistedOwners() => _bot.BlacklistedServerOwners;

        private struct BotConfig {
            public string Token;
            public string CommandPrefix;
            public ulong Owner;
            public string Game;
            public string Streamer;
            public uint SuccessEmbedColor;
            public uint ErrorEmbedColor;
            public bool LogAllCommands;
            public JoinLeaveLog JoinLeaveLog;
            public ulong[] BlacklistedServerOwners;
        }
    }
}