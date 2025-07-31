using Volte.Systems.Database;
using Volte.Systems.Database.Entities;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Entities;

public sealed class MessageReceivedEventArgs : EventArgs
{
    public SocketUserMessage Message { get; }
    public VolteContext Context { get; }
    public GuildDataV2 Data { get; }

    public MessageReceivedEventArgs(SocketMessage s, IServiceProvider provider)
    {
        Message = s.Cast<SocketUserMessage>() ?? throw new ArgumentException($"{nameof(s)} is not a SocketUserMessage; aborting EventArgs construction.");
        Context = new(s, provider);
        Data = provider.Get<DatabaseService>().GetData(Context.Guild);
    }
}