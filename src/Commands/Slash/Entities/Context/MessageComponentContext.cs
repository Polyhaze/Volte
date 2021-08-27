using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace Volte.Commands.Slash
{
    public class MessageComponentContext : InteractionContext<SocketMessageComponent>
    {
        public MessageComponentContext(SocketMessageComponent backing, IServiceProvider provider)
            : base(backing, provider) { }

        public string CustomId => Backing.Data.CustomId;

        public HashSet<string> SelectedMenuOptions
            => Backing.Data.Values?.ToHashSet() ?? new HashSet<string>();
    }
}