namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketRoleDeleteAuditLogData>(ActionType.RoleDeleted)]
    private static void RoleDeleted(AuditLogContext<SocketRoleDeleteAuditLogData> ctx)
    {
        ctx.Message.WithColor(Color.DarkRed)
            .WithTitle("Role deleted")
            .AddField("Target", $"{ctx.Data.Properties.Name} ({ctx.Data.RoleId})");
    }

}