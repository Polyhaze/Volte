using Discord;
using Discord.WebSocket;

namespace Volte.Data.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs
    {
        public IGuild Guild { get; }
        public DiscordSocketClient Client { get; }

        public JoinedGuildEventArgs(SocketGuild guild)
        {
            Guild = guild;
        }
    }
}