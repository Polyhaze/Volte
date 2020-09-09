using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gommon;
using JetBrains.Annotations;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Core
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
            IgnoreNullValues = false
        };

        private static bool IsValidConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                if (File.ReadAllText(ConfigFilePath).IsNullOrEmpty()) return false;
                return true;
            }
            return false;
        }

        public static bool StartupChecks()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Console.WriteLine($"The \"{DataDirectory}\" directory didn't exist, so I created it for you. Please fill in the configuration!", Color.Red);
                Directory.CreateDirectory(DataDirectory);
                //99.9999999999% of the time the config also won't exist if this block is reached
                //if the config does exist when this block is reached, feel free to become the lead developer of this project
            }
            
            if (!CreateIfAbsent())
            {
                Console.WriteLine($"Please fill in the configuration located at \"{ConfigFilePath}\"; restart me when you've done so.", Color.Crimson);
                return false;
            }

            return true;
        }
        
        public static bool CreateIfAbsent()
        {
            if (IsValidConfig()) return true;
            _configuration = new BotConfig
            {
                Tokens = new Tokens(),
                CommandPrefix = "$",
                Owner = 0,
                Game = "game here",
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
                    JsonSerializer.Serialize(_configuration, JsonOptions));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return false;
        }

        public static void Load()
        {
            _ = CreateIfAbsent();
            if (IsValidConfig())
                _configuration = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText(ConfigFilePath), JsonOptions);                    
        }

        public static bool Reload([NotNull] IServiceProvider provider)
        {
            var logger = provider.Get<LoggingService>();
            try
            {
                Load();
                return true;
            }
            catch (JsonException e)
            {
                logger.Exception(e);
                return false;
            }
        }

        public static bool IsValidDiscordToken() 
            => !(Tokens.Discord.IsNullOrEmpty() || Tokens.Discord.Equals("discord bot token here"));

        public static Tokens Tokens => _configuration.Tokens;

        public static string CommandPrefix => _configuration.CommandPrefix;

        public static ulong Owner => _configuration.Owner;

        public static string Game => _configuration.Game;

        public static string Streamer => _configuration.Streamer;

        public static bool EnableDebugLogging => _configuration.EnableDebugLogging;

        public static string FormattedStreamUrl => $"https://twitch.tv/{Streamer}";

        public static int SuccessColor => _configuration.SuccessEmbedColor.Cast<int>();

        public static int ErrorColor => _configuration.ErrorEmbedColor.Cast<int>();

        public static bool LogAllCommands => _configuration.LogAllCommands;

        public static GuildLogging GuildLogging => _configuration.GuildLogging;

        public static IEnumerable<ulong> BlacklistedOwners => _configuration.BlacklistedGuildOwners;

        public static EnabledFeatures EnabledFeatures => _configuration.EnabledFeatures;
        
        // ReSharper disable MemberHidesStaticFromOuterClass
        private class BotConfig
        {
            [JsonPropertyName("tokens")]
            public Tokens Tokens { get; set; }

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