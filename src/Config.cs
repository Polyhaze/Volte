using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;

namespace Volte
{
    public static class Config
    {
        public const string DataDirectory = "data";
        public const string ConfigFilePath = DataDirectory + "/volte.json";
        private static BotConfig _configuration;

        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = false,
            AllowTrailingCommas = true
        };

        private static bool IsValidConfig() 
            => File.Exists(ConfigFilePath) && !File.ReadAllText(ConfigFilePath).IsNullOrEmpty();

        public static bool StartupChecks()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Logger.Error(LogSource.Volte,
                    $"The \"{DataDirectory}\" directory didn't exist, so I created it for you. Please fill in the configuration!");
                Directory.CreateDirectory(DataDirectory);
                //99.9999999999% of the time the config also won't exist if this block is reached
                //if the config does exist when this block is reached, feel free to become the lead developer of this project
            }

            if (CreateIfAbsent()) return true;
            Logger.Error(LogSource.Volte,
                $"Please fill in the configuration located at \"{ConfigFilePath}\"; restart me when you've done so.");
            return false;

        }
        
        public static bool CreateIfAbsent()
        {
            if (IsValidConfig()) return true;
            _configuration = new BotConfig
            {
                Token = "token here",
                SentryDsn = "",
                CommandPrefix = "$",
                Owner = 0,
                Game = "game here",
                Streamer = "streamer here",
                EnableDebugLogging = false,
                SuccessEmbedColor = 0x7000FB,
                ErrorEmbedColor = 0xFF0000,
                LogAllCommands = true,
                GuildLogging = new GuildLogging(),
                BlacklistedGuildOwners = new HashSet<ulong>(),
                EnabledFeatures = new EnabledFeatures()
            };
            try
            {
                File.WriteAllText(ConfigFilePath,
                    JsonSerializer.Serialize(_configuration, JsonOptions));
            }
            catch (Exception e)
            {
                Logger.Error(LogSource.Volte, e.Message, e);
            }

            return false;
        }

        public static void Load()
        {
            _ = CreateIfAbsent();
            if (IsValidConfig())
                _configuration = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText(ConfigFilePath), JsonOptions);                    
        }

        public static bool Reload()
        {
            try
            {
                _configuration = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText(ConfigFilePath));
                return true;
            }
            catch (JsonException e)
            {
                Logger.Exception(e);
                return false;
            }
        }

        public static (ActivityType Type, string Name, string Streamer) ParseActivity()
        {
            var split = Game.Split(" ");
            var title = split.Skip(1).Join(" ");
            if (split[0].ToLower() is "streaming") title = split.Skip(2).Join(" ");
            return split.First().ToLower() switch
            {
                "playing" => (ActivityType.Playing, title, null),
                "listeningto" => (ActivityType.Listening, title, null),
                "listening" => (ActivityType.Listening, title, null),
                "streaming" => (ActivityType.Streaming, title, split[1]),
                "watching" => (ActivityType.Watching, title, null),
                _ => (ActivityType.Playing, Game, null)
            };
        }

        public static bool IsValidToken() 
            => !(Token.IsNullOrEmpty() || Token.Equals("token here"));

        public static string Token => _configuration.Token;

        public static string CommandPrefix => _configuration.CommandPrefix;

        public static string SentryDsn => _configuration.SentryDsn;

        public static ulong Owner => _configuration.Owner;

        public static string Game => _configuration.Game;

        public static string Streamer => _configuration.Streamer;

        public static bool EnableDebugLogging => _configuration.EnableDebugLogging;

        public static string FormattedStreamUrl => $"https://twitch.tv/{Streamer}";

        public static uint SuccessColor => _configuration.SuccessEmbedColor;

        public static uint ErrorColor => _configuration.ErrorEmbedColor;

        public static bool LogAllCommands => _configuration.LogAllCommands;

        public static GuildLogging GuildLogging => _configuration.GuildLogging;

        public static HashSet<ulong> BlacklistedOwners => _configuration.BlacklistedGuildOwners;

        public static EnabledFeatures EnabledFeatures => _configuration.EnabledFeatures;
        
        // ReSharper disable MemberHidesStaticFromOuterClass
        private struct BotConfig
        {
            [JsonPropertyName("discord_token")]
            public string Token { get; set; }
            
            [JsonPropertyName("sentry_dsn")]
            public string SentryDsn { get; set; }

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
            public HashSet<ulong> BlacklistedGuildOwners { get; set; }

            [JsonPropertyName("enabled_features")]
            public EnabledFeatures EnabledFeatures { get; set; }
        }
    }
}