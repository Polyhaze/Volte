using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs : System.EventArgs
    {
        public SocketGuild Guild { get; }

        public JoinedGuildEventArgs(SocketGuild guild) => Guild = guild.Cast<SocketGuild>();
    }
}