using Discord.WebSocket;

namespace Volte.Core.Entities
{
    public sealed class UserJoinedEventArgs : System.EventArgs
    {
        public SocketGuildUser User { get; }
        public SocketGuild Guild { get; }

        public UserJoinedEventArgs(SocketGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }
    }
}