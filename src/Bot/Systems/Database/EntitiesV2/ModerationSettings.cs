using ActionType = Volte.Systems.UserFilter.ActionType;

namespace Volte.Systems.Database.EntitiesV2;

public class ModerationSettings
{
    public ulong ActionLogChannel { get; set; }

    public ulong ModRole { get; set; }

    public ulong SecondaryModRole { get; set; }

    public ulong AdminRole { get; set; }

    public bool CheckAccountAge { get; set; }
    
    public bool AttachmentSpamDetection { get; set; }

    public ActionType AttachmentSpamAction { get; set; } = ActionType.Kick;
    
    public string AttachmentSpamReason { get; set; }

    public ulong UnverifiedRole { get; set; }

    public ulong VerifiedRole { get; set; }

    public bool ShowResponsibleModerator { get; set; }
}