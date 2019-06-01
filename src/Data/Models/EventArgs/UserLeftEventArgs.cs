using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models.EventArgs
{
    public sealed class UserLeftEventArgs : System.EventArgs
    {
        public IGuildUser User { get; }
        public IGuild Guild { get; }

        public UserLeftEventArgs(IGuildUser user)
        {
            User = user;
            Guild = user.Guild;
        }
    }
}