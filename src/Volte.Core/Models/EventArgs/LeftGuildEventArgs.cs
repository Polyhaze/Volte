using DSharpPlus.Entities;

namespace Volte.Core.Models.EventArgs
{
    public sealed class LeftGuildEventArgs : System.EventArgs
    {
        public DiscordGuild Guild { get; }

        public LeftGuildEventArgs(DiscordGuild guild) 
            => Guild = guild;
    }
}