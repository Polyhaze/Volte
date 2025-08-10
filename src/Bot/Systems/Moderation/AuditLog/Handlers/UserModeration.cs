namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketBanAuditLogData>(ActionType.Ban)]
    private static async Task Ban(AuditLogContext<SocketBanAuditLogData> ctx)
    {
        var target = await ctx.Data.Target.GetOrDownloadAsync();

        ctx.Message.WithColor(Color.DarkRed)
            .WithTitle("User banned")
            .WithThumbnailUrl(target.GetDisplayAvatarUrl())
            .AddField("Target", $"{target.Username} ({target.Id})")
            .AddField("Reason", Format.Code(ctx.Entry.Reason, string.Empty));
    }
}