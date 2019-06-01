using Discord;
using Discord.WebSocket;
using Volte.Core;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models.EventArgs
{
    public sealed class LeftGuildEventArgs
    {
        public IGuild Guild { get; }

        public LeftGuildEventArgs(SocketGuild guild)
        {
            Guild = guild;
        }
    }
}