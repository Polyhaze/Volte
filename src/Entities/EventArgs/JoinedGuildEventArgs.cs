using System;
using Discord;
using Discord.WebSocket;

namespace Volte.Entities
{
    public sealed class JoinedGuildEventArgs : EventArgs
    {
        public SocketGuild Guild { get; }

        public JoinedGuildEventArgs(SocketGuild guild) => Guild = guild;
    }
}