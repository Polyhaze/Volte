using DSharpPlus.Entities;

namespace Volte.Core.Models.EventArgs
{
    public sealed class JoinedGuildEventArgs : System.EventArgs
    {
        public DiscordGuild Guild { get; }

        public JoinedGuildEventArgs(DiscordGuild guild) => Guild = guild;
    }
}