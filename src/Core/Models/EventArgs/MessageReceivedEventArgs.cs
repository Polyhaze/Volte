using System;
using DSharpPlus.Entities;
using Gommon;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Core.Models.EventArgs
{
    public sealed class MessageReceivedEventArgs : System.EventArgs
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