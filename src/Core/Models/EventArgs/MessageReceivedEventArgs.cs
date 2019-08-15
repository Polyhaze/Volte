using System;
using Discord.WebSocket;
using Gommon;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Core.Models.EventArgs
{
    public sealed class MessageReceivedEventArgs : System.EventArgs
    {
        private readonly DatabaseService _db;
        public SocketUserMessage Message { get; }
        public VolteContext Context { get; }
        public GuildData Data { get; }

        public MessageReceivedEventArgs(SocketMessage s, IServiceProvider provider)
        {
            Message = s.Cast<SocketUserMessage>() ?? throw new ArgumentException($"{nameof(s)} is not a SocketUserMessage; aborting event handler call.");
            provider.Get(out _db);
            Context = VolteContext.Create(s, provider);
            Data = _db.GetData(Context.Guild);
        }
    }
}