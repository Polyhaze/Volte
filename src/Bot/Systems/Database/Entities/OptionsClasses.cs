using System.Text.Json.Serialization;

namespace Volte.Systems.Database.Entities;

public sealed class ModerationOptions
{
    [JsonPropertyName("mod_log_channel")]
    public ulong ModActionLogChannel { get; set; }

    [JsonPropertyName("mod_role")]
    public ulong ModRole { get; set; }

    [JsonPropertyName("admin_role")]
    public ulong AdminRole { get; set; }
        
    [JsonPropertyName("check_account_age_on_join")]
    public bool CheckAccountAge { get; set; }
        
    [JsonPropertyName("unverified_role")]
    public ulong UnverifiedRole { get; set; }
        
    [JsonPropertyName("verified_role")]
    public ulong VerifiedRole { get; set; }
        
    [JsonPropertyName("show_moderator")]
    public bool ShowResponsibleModerator { get; set; }
        
    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}

public sealed class WelcomeOptions
{
    [JsonPropertyName("welcome_channel")]
    public ulong WelcomeChannel { get; set; }

    [JsonPropertyName("welcome_message")]
    public string WelcomeMessage { get; set; }

    [JsonPropertyName("leaving_message")]
    public string LeavingMessage { get; set; }

    [JsonPropertyName("welcome_color")]
    public uint WelcomeColor { get; set; }

    [JsonPropertyName("welcome_dm_message")]
    public string WelcomeDmMessage { get; set; }

    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}

public sealed class StarboardOptions
{
    [JsonPropertyName("starboard_channel")]
    public ulong StarboardChannel { get; set; }
        
    [JsonPropertyName("starboard_enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("number_of_required_stars")]
    public int StarsRequiredToPost { get; set; } = 1;
        
    [JsonPropertyName("delete_invalid_stars")]
    public bool DeleteInvalidStars { get; set; }
}