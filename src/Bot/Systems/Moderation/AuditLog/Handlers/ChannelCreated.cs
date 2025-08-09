namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    [AuditLogHandler<SocketChannelCreateAuditLogData>(ActionType.ChannelCreated)]
    private static void ChannelCreated(AuditLogContext<SocketChannelCreateAuditLogData> ctx) => ctx.Message
            .WithTitle("Channel created")
            .AddField("Name", Format.Code(ctx.Data.ChannelName))
            .AddNullableStringField("Topic", ctx.Data.Topic,
                topic => Format.Code(topic, string.Empty)
            );
}