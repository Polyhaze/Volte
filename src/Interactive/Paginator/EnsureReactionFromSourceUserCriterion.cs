using System.Threading.Tasks;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    internal class EnsureReactionFromSourceUserCriterion : ICriterion<SocketReaction>
    {
        public ValueTask<bool> CheckAsync(SocketUserMessage message, SocketReaction parameter) 
            => new ValueTask<bool>(parameter.UserId == message.Author.Id);
    }
}
