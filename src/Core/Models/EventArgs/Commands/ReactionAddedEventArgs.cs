using Discord;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Models.EventArgs
{
    public class ReactionAddedEventArgs : System.EventArgs
    {
        public Cacheable<IUserMessage, ulong> Message { get; }
        public SocketGuild Guild { get; }
        public SocketTextChannel Channel { get; }
        public SocketReaction Reaction { get; }

        public ReactionAddedEventArgs(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            Message = message;
            Channel = channel.Cast<SocketTextChannel>();
            Guild = Channel.Guild;
            Reaction = reaction;
        }
    }
}