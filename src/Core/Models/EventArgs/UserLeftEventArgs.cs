using Discord;
 
using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public sealed class UserLeftEventArgs : System.EventArgs
    {
        public UserLeftEventArgs(SocketGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }

        public SocketGuildUser User { get; }
        public SocketGuild Guild { get; }
    }
}