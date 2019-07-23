using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models.EventArgs
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