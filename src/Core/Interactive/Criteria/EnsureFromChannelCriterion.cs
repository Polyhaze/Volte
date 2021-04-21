using System.Threading.Tasks;
using Discord;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureFromChannelCriterion : ICriterion<IMessage>
    {
        private readonly ulong _channelId;

        public EnsureFromChannelCriterion(IMessageChannel channel)
            => _channelId = channel.Id;

        public ValueTask<bool> JudgeAsync(VolteContext sourceContext, IMessage parameter) 
            => new ValueTask<bool>(_channelId == parameter.Channel.Id);

    }
}
