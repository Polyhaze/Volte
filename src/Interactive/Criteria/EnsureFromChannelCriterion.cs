using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureFromChannelCriterion : ICriterion<IMessage>
    {
        private readonly ulong _channelId;

        public EnsureFromChannelCriterion(IMessageChannel channel)
            => _channelId = channel.Id;

        public ValueTask<bool> JudgeAsync(SocketUserMessage message, IMessage parameter) 
            => new ValueTask<bool>(_channelId == parameter.Channel.Id);

    }
}
