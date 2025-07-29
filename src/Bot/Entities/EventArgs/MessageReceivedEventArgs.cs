namespace Volte.Entities;

public sealed class MessageReceivedEventArgs : EventArgs
{
    public SocketUserMessage Message { get; }
    public VolteContext Context { get; }
    public GuildData Data { get; }

    public MessageReceivedEventArgs(SocketMessage s, IServiceProvider provider)
    {
        Message = s.Cast<SocketUserMessage>() ?? throw new ArgumentException($"{nameof(s)} is not a SocketUserMessage; aborting EventArgs construction.");
        Context = new(s, provider);
        Data = provider.Get<DatabaseService>().GetData(Context.Guild);
    }
}