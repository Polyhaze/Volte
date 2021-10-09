using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureSourceChannelCriterion : ICriterion<IMessage>
    {
        public ValueTask<bool> CheckAsync(SocketUserMessage message, IMessage parameter) 
            => new ValueTask<bool>(message.Channel.Id == parameter.Channel.Id);

    }
}
