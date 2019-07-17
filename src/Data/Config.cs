using System.IO;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Newtonsoft.Json;
using Volte.Data.Models;

namespace Volte.Data
{
    public sealed class Config
    {
        private const string ConfigFile = "data/config.json";
        private static BotConfig _bot;
        private static bool _valid = File.Exists(ConfigFile) && !File.ReadAllText(ConfigFile).IsNullOrEmpty();

        static Config()
        { 
            CreateIfNotExists();
            if (_valid)
                _bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(ConfigFile));
        }

        public static void CreateIfNotExists()
        {
            if (_valid) return;
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
                BlacklistedServerOwners = new ulong[] { },
                EnabledFeatures = new EnabledFeatures()
            };
            File.WriteAllText(ConfigFile,
                JsonConvert.SerializeObject(_bot, Formatting.Indented));
        }

        public static string Token => _bot.Token;

        public static string CommandPrefix => _bot.CommandPrefix;

        public static ulong Owner => _bot.Owner;

        public static string Game => _bot.Game;

        public static string Streamer => _bot.Streamer;

        public static string FormattedStreamUrl => $"https://twitch.tv/{Streamer}";

        public static uint SuccessColor => _bot.SuccessEmbedColor;

        public static uint ErrorColor => _bot.ErrorEmbedColor;

        public static bool LogAllCommands => _bot.LogAllCommands;

        public static JoinLeaveLog JoinLeaveLog => _bot.JoinLeaveLog;

        public static ulong[] BlacklistedOwners => _bot.BlacklistedServerOwners;

        public static EnabledFeatures EnabledFeatures => _bot.EnabledFeatures;

        private struct BotConfig
        {
            [JsonProperty("discord_token")]
            public string Token;
            [JsonProperty("command_prefix")]
            public string CommandPrefix;
            [JsonProperty("bot_owner")]
            public ulong Owner;
            [JsonProperty("status_game")]
            public string Game;
            [JsonProperty("status_twitch_streamer")]
            public string Streamer;
            [JsonProperty("color_success")]
            public uint SuccessEmbedColor;
            [JsonProperty("color_error")]
            public uint ErrorEmbedColor;
            [JsonProperty("log_all_commands")]
            public bool LogAllCommands;
            [JsonProperty("join_leave_log")]
            public JoinLeaveLog JoinLeaveLog;
            [JsonProperty("blacklisted_guild_owners")]
            public ulong[] BlacklistedServerOwners;
            [JsonProperty("enabled_features")]
            public EnabledFeatures EnabledFeatures;
        }
    }
}