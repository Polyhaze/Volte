namespace Volte.Systems.Database.EntitiesV2;

public class ModerationSettings
{
    public Snowflake ActionLogChannel { get; set; }
    
    public Snowflake ModRole { get; set; }
    
    public Snowflake AdminRole { get; set; }
    
    public bool CheckAccountAge { get; set; }
    
    public Snowflake UnverifiedRole { get; set; }
    
    public Snowflake VerifiedRole { get; set; }
    
    public bool ShowResponsibleModerator { get; set; }
}