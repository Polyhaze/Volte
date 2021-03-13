using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Core.Entities
{
    public sealed class UserLeftEventArgs : System.EventArgs
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