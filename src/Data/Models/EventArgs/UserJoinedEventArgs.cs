using Discord;

namespace Volte.Data.Models.EventArgs
{
    public sealed class UserJoinedEventArgs : System.EventArgs
    {
        public IGuildUser User { get; }
        public IGuild Guild { get; }

        public UserJoinedEventArgs(IGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }
    }
}