using System.Threading.Tasks;
using Discord.Commands;
using Volte.Commands;

namespace Volte.Interactive
{
    public class EmptyCriterion<T> : ICriterion<T>
    {
        public ValueTask<bool> JudgeAsync(VolteContext sourceContext, T parameter)
            => new ValueTask<bool>(true);
    }
}
