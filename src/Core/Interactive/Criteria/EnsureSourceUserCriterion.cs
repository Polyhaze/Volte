using System.Threading.Tasks;
using Discord;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EnsureSourceUserCriterion : ICriterion<IMessage>
    {
        public Task<bool> JudgeAsync(VolteContext sourceContext, IMessage parameter)
        {
            return Task.FromResult(sourceContext.User.Id == parameter.Author.Id);
        }
    }
}
