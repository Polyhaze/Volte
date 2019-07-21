using Discord;
using Discord.WebSocket;

namespace Volte.Data.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs
    {
        public IGuild Guild { get; }
        public DiscordShardedClient Client { get; }

        public JoinedGuildEventArgs(SocketGuild guild)
        {
            Guild = guild;
        }
    }
}