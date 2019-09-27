using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Models.EventArgs
{
    public sealed class LeftGuildEventArgs : System.EventArgs
    {
        public SocketGuild Guild { get; }

        public LeftGuildEventArgs(SocketGuild guild) 
            => Guild = guild.Cast<SocketGuild>();
    }
}