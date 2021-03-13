using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Entities
{
    public sealed class JoinedGuildEventArgs : System.EventArgs
    {
        public SocketGuild Guild { get; }

        public JoinedGuildEventArgs(SocketGuild guild) => Guild = guild;
    }
}