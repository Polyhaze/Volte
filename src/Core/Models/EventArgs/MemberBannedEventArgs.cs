using Discord.WebSocket;

namespace Volte.Core.Models.EventArgs
{
    public class MemberBannedEventArgs : System.EventArgs
    {
        public SocketUser User { get; }
        public SocketGuild Guild { get; }

        public MemberBannedEventArgs(SocketUser user, SocketGuild guild)
        {
            User = user;
            Guild = guild;
        }
    }
}