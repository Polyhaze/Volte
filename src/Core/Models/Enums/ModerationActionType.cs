namespace BrackeysBot
{
    public enum ModerationActionType
    {
        [Description("Warned user")]
        Warn,
        [Description("Cleared infractions")]
        ClearInfractions,
        [Description("Deleted infraction")]
        DeletedInfraction,

        [Description("Muted user")]
        Mute,
        [Description("Tempmuted user")]
        TempMute,
        [Description("Unmuted user")]
        Unmute,

        [Description("Banned user")]
        Ban,
        [Description("Tempbanned user")]
        TempBan,
        [Description("Unbanned user")]
        Unban,
        
        [Description("Set slowmode")]
        SlowMode,
        [Description("Lockdown")]
        Lockdown,

        [Description("Cleared messages")]
        ClearMessages,

        [Description("Kicked user")]
        Kick,

        [Description("Filtered word")]
        Filtered
    }
}
