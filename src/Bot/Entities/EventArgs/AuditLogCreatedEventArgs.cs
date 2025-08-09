namespace Volte.Entities;

public class AuditLogCreatedEventArgs
{
    public SocketAuditLogEntry Entry { get; }
    public SocketGuild Guild { get; }

    public AuditLogCreatedEventArgs(SocketAuditLogEntry entry, SocketGuild guild)
    {
        Entry = entry;
        Guild = guild;
    }
}