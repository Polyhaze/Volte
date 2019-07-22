using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Data.Models.EventArgs
{
    public sealed class LeftGuildEventArgs : System.EventArgs
    {
        public IGuild Guild { get; }

        public LeftGuildEventArgs(SocketGuild guild)
        {
            Guild = guild.Cast<IGuild>();
        }
    }
}