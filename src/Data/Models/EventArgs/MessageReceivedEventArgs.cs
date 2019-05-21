using Discord;
using Discord.WebSocket;
using Volte.Commands;
using Volte.Core;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models.EventArgs
{
    public sealed class MessageReceivedEventArgs : System.EventArgs
    {
        private readonly DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();
        public IUserMessage Message { get; }
        public VolteContext Context { get; }
        public GuildData Data { get; }

        public MessageReceivedEventArgs(SocketMessage s)
        {
            Message = s as IUserMessage;
            Context = new VolteContext(VolteBot.Client, Message, VolteBot.ServiceProvider);
            Data = _db.GetData(Context.Guild);
        }
    }
}