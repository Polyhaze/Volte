using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gommon;
using Volte.Core.Models.BotConfig;
using Volte.Services;

namespace Volte.Core
{
    public static class Config
    {
        public const string DataDirectory = "data";
        public const string ConfigFilePath = DataDirectory + "/volte.json";
        private static BotConfig _configuration;

        private static readonly bool IsValidConfig =
            File.Exists(ConfigFilePath) && !File.ReadAllText(ConfigFilePath).IsNullOrEmpty();

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
                File.WriteAllText(ConfigFilePath,
                    JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return false;
        }

        public static void Load()
        {
            _ = CreateIfNotExists();
            if (IsValidConfig)
                _configuration = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText(ConfigFilePath), new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                });                    
        }

        public static bool Reload(IServiceProvider provider)
        {
            provider.Get<LoggingService>(out var logger);
            try
            {
                _configuration = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText(ConfigFilePath));
                return true;
            }
            catch (JsonException e)
            {
                logger.Exception(e);
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
        private class BotConfig
        {
            [JsonPropertyName("discord_token")]
            public string Token { get; set; }

            [JsonPropertyName("command_prefix")]
            public string CommandPrefix { get; set; }

            [JsonPropertyName("bot_owner")]
            public ulong Owner { get; set; }

            [JsonPropertyName("status_game")]
            public string Game { get; set; }

            [JsonPropertyName("status_twitch_streamer")]
            public string Streamer { get; set; }

            [JsonPropertyName("enable_debug_logging")]
            public bool EnableDebugLogging { get; set; }

            [JsonPropertyName("color_success")]
            public uint SuccessEmbedColor { get; set; }

            [JsonPropertyName("color_error")]
            public uint ErrorEmbedColor { get; set; }

            [JsonPropertyName("log_all_commands")]
            public bool LogAllCommands { get; set; }

            [JsonPropertyName("guild_logging")]
            public GuildLogging GuildLogging { get; set; }

            [JsonPropertyName("blacklisted_guild_owners")]
            public ulong[] BlacklistedGuildOwners { get; set; }

            [JsonPropertyName("enabled_features")]
            public EnabledFeatures EnabledFeatures { get; set; }
        }
    }
}