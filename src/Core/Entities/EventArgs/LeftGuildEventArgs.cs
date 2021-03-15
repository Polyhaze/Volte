using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Entities
{
    public sealed class LeftGuildEventArgs : System.EventArgs
    {
        public SocketGuild Guild { get; }

        public LeftGuildEventArgs(SocketGuild guild) => Guild = guild;
    }
}