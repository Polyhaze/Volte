using Discord;
using Discord.WebSocket;
using System;

namespace Volte.Core.Entities
{
    public sealed class UserLeftEventArgs : EventArgs
    {
        public SocketGuildUser User { get; }
        public SocketGuild Guild { get; }

        public UserLeftEventArgs(SocketGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }
    }
}