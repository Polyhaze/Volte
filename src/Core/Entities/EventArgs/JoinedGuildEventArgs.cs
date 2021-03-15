using Discord;
using Discord.WebSocket;
using System;

namespace Volte.Core.Entities
{
    public sealed class JoinedGuildEventArgs : EventArgs
    {
        public SocketGuild Guild { get; }

        public JoinedGuildEventArgs(SocketGuild guild) => Guild = guild;
    }
}