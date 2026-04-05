using System.Text.Json.Serialization;

namespace Volte.Systems.Configuration;

public static class Config
{
    private static IVolteConfig _configuration;

    public static readonly JsonSerializerOptions JsonOptions = CreateSerializerOptions(true);
    public static readonly JsonSerializerOptions MinifiedJsonOptions = CreateSerializerOptions(false);

    public static readonly FilePath Path = FilePath.Data / "volte.json";

    private static JsonSerializerOptions CreateSerializerOptions(bool writeIndented)
        => new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = writeIndented,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true
        }.Apply(opt =>
        {
            opt.Converters.Add(new Snowflake.JsonConverter());
        });
    

    private static bool IsValidConfig() 
        => Path.ExistsAsFile && !Path.ReadAllText().IsNullOrEmpty();

    public static bool StartupChecks<TConfig>() where TConfig : IVolteConfig, new()
    {
        if (!FilePath.Data.ExistsAsDirectory)
        {
            Error(LogSource.Volte,
                $"The \"{FilePath.Data}\" directory didn't exist, so I created it for you. Please fill in the configuration!");
            FilePath.Data.CreateAsDirectory();
            //99.9999999999% of the time the config also won't exist if this block is reached
            //if the config does exist when this block is reached, feel free to become the lead developer of this project
        }

        if (CreateIfAbsent<TConfig>()) return true;
        Error(LogSource.Volte,
            $"Please fill in the configuration located at \"{Path}\"; restart me when you've done so.");
        return false;

    }
        
    public static bool CreateIfAbsent<TConfig>() where TConfig : IVolteConfig, new()
    {
        if (IsValidConfig()) return true;
        _configuration = new TConfig
        {
            Token = "token here",
            SentryDsn = string.Empty,
            CommandPrefix = "$",
            Owner = 0,
            Game = "game here",
            Streamer = "streamer here",
            EnableDebug = false,
            SuccessEmbedColor = 0x7000FB,
            ErrorEmbedColor = 0xFF0000,
            LogAllCommands = true,
            BlacklistedGuildOwners = [],
            EnabledFeatures = new()
        };
        
        try
        {
            Path.WriteAllText(JsonSerializer.Serialize(_configuration, JsonOptions));
        }
        catch (Exception e)
        {
            Error(LogSource.Volte, e.Message, e);
        }

        return false;
    }

    public static void Load<TConfig>() where TConfig : IVolteConfig, new()
    {
        _ = CreateIfAbsent<TConfig>();
        if (IsValidConfig())
            _configuration = JsonSerializer.Deserialize<TConfig>(Path.ReadAllText(), JsonOptions);                    
    }

    public static bool Reload<TConfig>() where TConfig : IVolteConfig
    {
        try
        {
            _configuration = JsonSerializer.Deserialize<TConfig>(Path.ReadAllText(), JsonOptions);
            return true;
        }
        catch (JsonException e)
        {
            Error(e);
            return false;
        }
    }
    
    public static bool Edit<TConfig>(TConfig newConfig) where TConfig : IVolteConfig
    {
        try
        {
            Path.WriteAllText(JsonSerializer.Serialize(newConfig));
            Reload<TConfig>();
            return true;
        }
        catch (JsonException e)
        {
            Error(e);
            return false;
        }
    }

    public static (ActivityType Type, string Name, string Streamer) ParseActivity()
    {
        var split = Game.Split(" ");
        var title = split.Skip(1).JoinToString(" ");
        if (split[0].ToLower() is "streaming") title = split.Skip(2).JoinToString(" ");
        return split[0].ToLower() switch
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

    public static Snowflake Owner => _configuration.Owner;

    public static string Game => _configuration.Game;

    public static string Streamer => _configuration.Streamer;

    public static bool DebugEnabled => _configuration?.EnableDebug ?? false;
    
    public static string[] IgnoredDebugLines => _configuration?.IgnoredDebugMessages ?? [];

    public static string FormattedStreamUrl => $"https://twitch.tv/{Streamer}";

    public static uint SuccessColor => _configuration.SuccessEmbedColor;

    public static uint ErrorColor => _configuration.ErrorEmbedColor;

    public static bool LogAllCommands => _configuration.LogAllCommands;

    public static HashSet<Snowflake> BlacklistedOwners => _configuration.BlacklistedGuildOwners;

    public static EnabledFeatures EnabledFeatures => _configuration?.EnabledFeatures;
}