using System;
using DSharpPlus.Entities;
using Gommon;
using Volte.Commands;
using Volte.Services;

namespace Volte.Core.Entities
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public DiscordMessage Message { get; }
        public VolteContext Context { get; }
        public GuildData Data { get; }

        public MessageReceivedEventArgs(DiscordMessage s, IServiceProvider provider)
        {
            Message = s;
            Context = VolteContext.Create(s, provider);
            Data = provider.Get<DatabaseService>().GetData(Context.Guild);
        }
    }
}