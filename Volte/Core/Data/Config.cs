using System.IO;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Data
{
    public class Config
    {
        private const string ConfigFile = "data/config.json";
        private static BotConfig _bot;

        static Config()
        {
            CreateIfNotExists();
            if (File.Exists(ConfigFile) && !File.ReadAllText(ConfigFile).IsNullOrEmpty())
                _bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(ConfigFile));
        }

        public static void CreateIfNotExists()
        {
            if (File.Exists(ConfigFile) && !File.ReadAllText(ConfigFile).IsNullOrEmpty()) return;
            var logger = VolteBot.GetRequiredService<LoggingService>();
            logger.Log(LogSeverity.Warning, LogSource.Volte,
                "config.json didn't exist or was empty. Created it for you.").GetAwaiter().GetResult();
            _bot = new BotConfig
            {
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

        public static string Token => _bot.Token;

        public static string CommandPrefix => _bot.CommandPrefix;

        public static ulong Owner => _bot.Owner;

        public static string Game => _bot.Game;

        public static string Streamer => _bot.Streamer;

        public static uint SuccessColor => _bot.SuccessEmbedColor;

        public static uint ErrorColor => _bot.ErrorEmbedColor;

        public static bool LogAllCommands => _bot.LogAllCommands;

        public static JoinLeaveLog JoinLeaveLog => _bot.JoinLeaveLog;

        public static ulong[] BlacklistedOwners =>_bot.BlacklistedServerOwners;

        private struct BotConfig
        {
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