using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Core.Models.EventArgs
{
    public sealed class MessageReceivedEventArgs : System.EventArgs
    {
        private readonly DatabaseService _db;

        public MessageReceivedEventArgs(SocketMessage s, ServiceProvider provider)
        {
            Message = s.Cast<SocketUserMessage>();
            provider.Get(out _db);
            provider.Get<DiscordShardedClient>(out var client);
            Context = new VolteContext(client, Message, provider);
            Data = _db.GetData(Context.Guild);
        }

        public SocketUserMessage Message { get; }
        public VolteContext Context { get; }
        public GuildData Data { get; }
    }
}