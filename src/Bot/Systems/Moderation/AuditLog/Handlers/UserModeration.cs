namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketBanAuditLogData>(ActionType.Ban)]
    private static async Task Ban(AuditLogContext<SocketBanAuditLogData> ctx)
    {
        var reason = string.IsNullOrEmpty(ctx.Entry.Reason) ? "None provided" : ctx.Entry.Reason;

        var target = await ctx.Data.Target.GetOrDownloadAsync();

        ctx.Message.WithColor(Color.DarkRed)
            .WithTitle("User banned")
            .WithThumbnailUrl(target.GetDisplayAvatarUrl())
            .AddField("Target", $"{target.Username} ({target.Id})")
            .AddField("Reason", Format.Code(reason, string.Empty));
    }

    [AuditLogHandler<SocketKickAuditLogData>(ActionType.Kick)]
    private static async Task Kick(AuditLogContext<SocketKickAuditLogData> ctx)
    {
        var reason = string.IsNullOrEmpty(ctx.Entry.Reason) ? "None provided" : ctx.Entry.Reason;

        var target = await ctx.Data.Target.GetOrDownloadAsync();

        ctx.Message.WithColor(Color.DarkRed)
            .WithTitle("User kicked")
            .WithThumbnailUrl(target.GetDisplayAvatarUrl())
            .AddField("Target", $"{target.Username} ({target.Id})")
            .AddField("Reason", Format.Code(reason, string.Empty));
    }

    [AuditLogHandler<SocketUnbanAuditLogData>(ActionType.Unban)]
    private static async Task Unban(AuditLogContext<SocketUnbanAuditLogData> ctx)
    {
        var reason = string.IsNullOrEmpty(ctx.Entry.Reason) ? "None provided" : ctx.Entry.Reason;

        var target = await ctx.Data.Target.GetOrDownloadAsync();

        ctx.Message.WithColor(Color.DarkRed)
            .WithTitle("User kicked")
            .WithThumbnailUrl(target.GetDisplayAvatarUrl())
            .AddField("Target", $"{target.Username} ({target.Id})")
            .AddField("Reason", Format.Code(reason, string.Empty));
    }
}