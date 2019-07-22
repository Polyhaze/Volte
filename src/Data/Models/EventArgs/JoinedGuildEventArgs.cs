using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Data.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs : System.EventArgs
    {
        public IGuild Guild { get; }
        public DiscordShardedClient Client { get; }

        public JoinedGuildEventArgs(SocketGuild guild)
        {
            Guild = guild.Cast<IGuild>();
        }
    }
}