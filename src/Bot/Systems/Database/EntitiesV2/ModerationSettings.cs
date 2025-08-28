namespace Volte.Systems.Database.EntitiesV2;

public class ModerationSettings
{
    public ulong ActionLogChannel { get; set; }
    
    public ulong ModRole { get; set; }
    
    public ulong AdminRole { get; set; }
    
    public bool CheckAccountAge { get; set; }
    
    public ulong UnverifiedRole { get; set; }
    
    public ulong VerifiedRole { get; set; }
    
    public bool ShowResponsibleModerator { get; set; }
}