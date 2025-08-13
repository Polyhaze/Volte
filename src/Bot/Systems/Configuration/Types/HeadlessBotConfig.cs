using System.Text.Json.Serialization;

namespace Volte.Systems.Configuration;

public struct HeadlessBotConfig : IVolteConfig
{
    [JsonPropertyName("discord_token")]
    public string Token { get; set; }
            
    [JsonPropertyName("sentry_dsn")]
    public string SentryDsn { get; set; }

    [JsonPropertyName("command_prefix")]
    public string CommandPrefix { get; set; }

    [JsonPropertyName("bot_owner")]
    public Snowflake Owner { get; set; }

    [JsonPropertyName("status_game")]
    public string Game { get; set; }

    [JsonPropertyName("status_twitch_streamer")]
    public string Streamer { get; set; }

    [JsonPropertyName("enable_debug_logging")]
    public bool EnableDebug { get; set; }
    
    [JsonPropertyName("ignored_debug_log_messages")]
    public string[] IgnoredDebugMessages { get; set; }

    [JsonPropertyName("color_success")]
    public uint SuccessEmbedColor { get; set; }

    [JsonPropertyName("color_error")]
    public uint ErrorEmbedColor { get; set; }

    [JsonPropertyName("log_all_commands")]
    public bool LogAllCommands { get; set; }

    [JsonPropertyName("blacklisted_guild_owners")]
    public HashSet<Snowflake> BlacklistedGuildOwners { get; set; }

    [JsonPropertyName("enabled_features")]
    public EnabledFeatures EnabledFeatures { get; set; }
}