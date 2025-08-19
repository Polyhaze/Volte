namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketGuildUpdateAuditLogData>(ActionType.GuildUpdated)]
    private static void GuildUpdated(AuditLogContext<SocketGuildUpdateAuditLogData> ctx)
    {
        
    }
}