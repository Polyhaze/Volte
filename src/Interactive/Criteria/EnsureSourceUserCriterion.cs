using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureSourceUserCriterion : ICriterion<IMessage>
    {
        public ValueTask<bool> JudgeAsync(SocketUserMessage message, IMessage parameter) 
            => new ValueTask<bool>(message.Author.Id == parameter.Author.Id);
    }
}
