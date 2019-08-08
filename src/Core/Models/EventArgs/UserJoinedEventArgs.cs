using Discord;
 
using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public sealed class UserJoinedEventArgs : System.EventArgs
    {
        public UserJoinedEventArgs(SocketGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }

        public SocketGuildUser User { get; }
        public SocketGuild Guild { get; }
    }
}