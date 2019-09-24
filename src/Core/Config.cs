using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Gommon;
using Newtonsoft.Json;
using Volte.Core.Models.BotConfig;
using Volte.Services;

namespace Volte.Core
{
    public static class Config
    {
        public const string DataDirectory = "data";
        public const string ConfigFile = DataDirectory + "/volte.json";
        private static BotConfig _configuration;

        private static readonly bool IsValidConfig =
            File.Exists(ConfigFile) && !File.ReadAllText(ConfigFile).IsNullOrEmpty();

        public static bool CreateIfNotExists()
        {
            if (IsValidConfig) return true;
            _configuration = new BotConfig
            {
                Token = "token here",
                CommandPrefix = "$",
                Owner = 0,
                Game = "in Volte V3 Code!",
                Streamer = "streamer here",
                EnableDebugLogging = false,
                SuccessEmbedColor = 0x7000FB,
                ErrorEmbedColor = 0xFF0000,
                LogAllCommands = true,
                GuildLogging = new GuildLogging(),
                BlacklistedGuildOwners = Array.Empty<ulong>(),
                EnabledFeatures = new EnabledFeatures()
            };
            try
            {
                File.WriteAllText(ConfigFile, 
                    JsonConvert.SerializeObject(_configuration, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return false;
        }

        public static void Load()
        {
            CreateIfNotExists();
            if (IsValidConfig)
                _configuration = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(ConfigFile));
        }

        public static bool Reload(IServiceProvider provider)
        {
            provider.Get<LoggingService>(out var logger);
            try
            {
                _configuration = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(ConfigFile));
                return true;
            }
            catch (JsonException e)
            {
                logger.LogException(e);
                return false;
            }
        }

        public static bool IsValidToken() 
            => !(Token.IsNullOrEmpty() || Token.Equals("token here"));

        public static string Token => _configuration.Token;

        public static string CommandPrefix => _configuration.CommandPrefix;

        public static ulong Owner => _configuration.Owner;

        public static string Game => _configuration.Game;

        public static string Streamer => _configuration.Streamer;

        public static bool EnableDebugLogging => _configuration.EnableDebugLogging;

        public static string FormattedStreamUrl => $"https://twitch.tv/{Streamer}";

        public static uint SuccessColor => _configuration.SuccessEmbedColor;

        public static uint ErrorColor => _configuration.ErrorEmbedColor;

        public static bool LogAllCommands => _configuration.LogAllCommands;

        public static GuildLogging GuildLogging => _configuration.GuildLogging;

        public static IEnumerable<ulong> BlacklistedOwners => _configuration.BlacklistedGuildOwners;

        public static EnabledFeatures EnabledFeatures => _configuration.EnabledFeatures;

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        private struct BotConfig
        {
            [JsonProperty("discord_token")]
            public string Token { get; internal set; }

            [JsonProperty("command_prefix")]
            public string CommandPrefix { get; internal set; }

            [JsonProperty("bot_owner")]
            public ulong Owner { get; internal set; }

            [JsonProperty("status_game")]
            public string Game { get; internal set; }

            [JsonProperty("status_twitch_streamer")]
            public string Streamer { get; internal set; }

            [JsonProperty("enable_debug_logging")]
            public bool EnableDebugLogging { get; internal set; }

            [JsonProperty("color_success")]
            public uint SuccessEmbedColor { get; internal set; }

            [JsonProperty("color_error")]
            public uint ErrorEmbedColor { get; internal set; }

            [JsonProperty("log_all_commands")]
            public bool LogAllCommands { get; internal set; }

            [JsonProperty("guild_logging")]
            public GuildLogging GuildLogging { get; internal set; }

            [JsonProperty("blacklisted_guild_owners")]
            public ulong[] BlacklistedGuildOwners { get; internal set; }

            [JsonProperty("enabled_features")]
            public EnabledFeatures EnabledFeatures { get; internal set; }
        }
    }
}