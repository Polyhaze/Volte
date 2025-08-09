namespace Volte.Systems.Database.EntitiesV2;

public class GuildSettings
{
    public ulong Autorole { get; set; }
    
    public string CommandPrefix { get; set; }
    
    public bool ReplyInline { get; set; }
    
    public bool EmbedTags { get; set; }
    
    public bool AutoQuoteMessageUrls { get; set; }
    
    public ModerationSettings Moderation { get; set; } = new();
    
    public StarboardSettings Starboard { get; set; } = new();
    
    public WelcomeSettings Welcome { get; set; } = new();
    
    public AuditLogArchiveSettings AuditLog { get; set; } = AuditLogArchiveSettings.Empty;
    
    public SettingsCollections Collections { get; set; } = new();
    
    public class SettingsCollections
    {
        public HashSet<ulong> SelfRoles { get; set; } = [];
        
        public HashSet<TagV2> Tags { get; set; } = [];
    }
}