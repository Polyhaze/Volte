namespace Volte.Systems.Moderation;

public class AuditLogArchiveMessageBuilder : CustomEmbedBuilder<AuditLogArchiveMessageBuilder>
{
    private readonly SocketTextChannel _channel;

    public AuditLogArchiveMessageBuilder(SocketUser causer, SocketTextChannel socketTextChannel)
    {
        _channel = socketTextChannel;
        this.WithAuthor(causer);
    }

#pragma warning disable CA1822
    public Task Done() => Task.CompletedTask;
#pragma warning restore CA1822

    public Task<IUserMessage> SendAsync() => SendToAsync(_channel);
}