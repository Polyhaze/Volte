namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketBotAddAuditLogData>(ActionType.BotAdded)]
    private static async Task BotAdded(AuditLogContext<SocketBotAddAuditLogData> ctx)
    {
        var target = await ctx.Data.Target.GetOrDownloadAsync();

        ctx.Message.WithColor(Color.Green)
            .WithTitle("Bot added")
            .WithThumbnailUrl(target.GetDisplayAvatarUrl())
            .AddField("Target", $"{target.Username} ({target.Id})");
    }
}