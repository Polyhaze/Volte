using System;
using Discord.WebSocket;
using Gommon;
using Volte.Commands;
using Volte.Services;

namespace Volte.Core.Entities
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public SocketUserMessage Message { get; }
        public VolteContext Context { get; }
        public GuildData Data { get; }

        public MessageReceivedEventArgs(SocketMessage s, IServiceProvider provider)
        {
            Message = s.Cast<SocketUserMessage>() ?? throw new ArgumentException($"{nameof(s)} is not a SocketUserMessage; aborting event handler call.");
            Context = VolteContext.Create(s, provider);
            Data = provider.Get<DatabaseService>().GetData(Context.Guild);
        }
    }
}