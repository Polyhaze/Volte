using System.Threading.Tasks;
using Discord;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureSourceChannelCriterion : ICriterion<IMessage>
    {
        public Task<bool> JudgeAsync(VolteContext sourceContext, IMessage parameter) 
            => Task.FromResult(sourceContext.Channel.Id == parameter.Channel.Id);

    }
}
